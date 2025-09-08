using System;
using System.Globalization;
using System.IO;
using MunicipalServicesMvc.Models;

namespace MunicipalServicesMvc.Services
{
    public class IssueStore
    {
        private readonly IssueList _issues = new IssueList();
        private readonly string _dataFilePath;
        private readonly object _lock = new object();
        private int _nextId = 1;

        public IssueStore(IHostEnvironment env)
        {
            var root = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(root);
            _dataFilePath = Path.Combine(root, "issues.txt");
            LoadFromFile();
        }

        public Issue Add(Issue issue)
        {
            lock (_lock)
            {
                issue.Id = _nextId++;
                _issues.Add(issue);
                AppendToFile(issue);
                return issue;
            }
        }

        public Issue? Get(int id)
        {
            lock (_lock) { return _issues.FindById(id); }
        }

        public IEnumerable<Issue> Recent(int n)
        {
            lock (_lock) { return _issues.Last(n); }
        }

        // ---------- Persistence: ISSUE|..., ATTACH|..., END ----------

        private static string Enc(string s) => Uri.EscapeDataString(s ?? "");
        private static string Dec(string s) => Uri.UnescapeDataString(s ?? "");

        private void AppendToFile(Issue issue)
        {
            using (var sw = new StreamWriter(new FileStream(_dataFilePath, File.Exists(_dataFilePath) ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                sw.Write("ISSUE|");
                sw.Write(issue.Id.ToString(CultureInfo.InvariantCulture)); sw.Write("|");
                sw.Write(issue.CreatedAt.ToString("o", CultureInfo.InvariantCulture)); sw.Write("|");
                sw.Write(Enc(issue.Location)); sw.Write("|");
                sw.Write(Enc(issue.Category)); sw.Write("|");
                sw.Write(Enc(issue.Description)); sw.Write("|");
                sw.WriteLine(Enc(issue.Status));

                foreach (var p in issue.Attachments) // enumerates our custom list
                {
                    sw.Write("ATTACH|");
                    sw.WriteLine(Enc(p));
                }
                sw.WriteLine("END");
            }
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_dataFilePath)) return;

            using (var sr = new StreamReader(new FileStream(_dataFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string? line;
                Issue? current = null;
                int maxId = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;

                    if (StartsWith(line, "ISSUE|"))
                    {
                        current = ParseIssueLine(line);
                        if (current.Id > maxId) maxId = current.Id;
                        continue;
                    }

                    if (StartsWith(line, "ATTACH|") && current != null)
                    {
                        var pathEnc = line.Substring(7); // after "ATTACH|"
                        current.Attachments.Add(Dec(pathEnc));
                        continue;
                    }

                    if (line == "END" && current != null)
                    {
                        _issues.Add(current);
                        current = null;
                    }
                }
                _nextId = maxId + 1;
            }
        }

        private static bool StartsWith(string s, string prefix)
        {
            if (s == null || prefix == null) return false;
            if (s.Length < prefix.Length) return false;
            for (int i = 0; i < prefix.Length; i++)
                if (s[i] != prefix[i]) return false;
            return true;
        }

        private static Issue ParseIssueLine(string line)
        {
            // ISSUE|Id|CreatedAtISO|LocationEnc|CategoryEnc|DescriptionEnc|StatusEnc
            int pos = 6; // after "ISSUE|"
            string idStr = ReadField(line, ref pos);
            string createdStr = ReadField(line, ref pos);
            string locEnc = ReadField(line, ref pos);
            string catEnc = ReadField(line, ref pos);
            string descEnc = ReadField(line, ref pos);
            string statusEnc = ReadField(line, ref pos);

            var issue = new Issue
            {
                Id = TryParseInt(idStr),
                CreatedAt = TryParseDate(createdStr),
                Location = Dec(locEnc),
                Category = Dec(catEnc),
                Description = Dec(descEnc),
                Status = Dec(statusEnc)
            };
            return issue;
        }

        private static string ReadField(string line, ref int pos)
        {
            if (pos >= line.Length) return "";
            int next = IndexOfChar(line, '|', pos);
            if (next == -1)
            {
                string tail = line.Substring(pos);
                pos = line.Length;
                return tail;
            }
            else
            {
                string field = line.Substring(pos, next - pos);
                pos = next + 1;
                return field;
            }
        }

        private static int IndexOfChar(string s, char c, int start)
        {
            for (int i = start; i < s.Length; i++)
                if (s[i] == c) return i;
            return -1;
        }

        private static int TryParseInt(string s)
        {
            int value = 0;
            bool neg = false;
            int i = 0;
            if (s.Length > 0 && s[0] == '-') { neg = true; i = 1; }
            for (; i < s.Length; i++)
            {
                char ch = s[i];
                if (ch < '0' || ch > '9') break;
                value = value * 10 + (ch - '0');
            }
            return neg ? -value : value;
        }

        private static DateTime TryParseDate(string s)
        {
            try { return DateTime.Parse(s, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind); }
            catch { return DateTime.UtcNow; }
        }
    }
}
