# Municipal Services MVC (ASP.NET Core 8)

A municipal services portal for South Africa that implements **Report Issues** with a **WhatsApp-first engagement strategy** and **custom data structures** (no `List<T>`, arrays, or LINQ in the storage/manipulation path).

---

## ✨ Features

- **Landing page** with three tasks:
  - **Report Issues** ✅ (implemented)
  - **Local Events & Announcements** 🔒 (disabled / “Coming Soon”)
  - **Service Request Status** 🔒 (disabled / “Coming Soon”)
- **Report Issue** form:
  - Location (textbox), Category (dropdown), Description (textarea)
  - Attachments (multiple files: images/PDFs/docs)
  - **Engagement cue:** progress bar + friendly helper text
- **WhatsApp-only engagement:** one-click **Share via WhatsApp Web** with a compact, prefilled ticket summary  
  (`Ticket: {Id} • Category: {Category} • Location: {Location} • Submitted: {YYYY-MM-DD HH:mm}`)
- **Custom data structures:**  
  - `IssueList` — singly linked list of `Issue` (append O(1), custom enumerator)  
  - `AttachmentList` — singly linked list of string paths
- **Manual persistence:** human-readable, line-based file (`App_Data/issues.txt`)

---

## 🧰 Prerequisites

- .NET 8 SDK (`dotnet --version` → 8.x)
- Windows/macOS/Linux
- (Optional) Visual Studio 2022 / VS Code / Rider

---

## 🚀 Run the app

### CLI
```bash
# from the repository root
dotnet restore
dotnet build
dotnet run
Open the printed URL (e.g., https://localhost:5xxx).

Visual Studio
Open the .csproj or solution.

Set configuration to Debug.

Press F5 (IIS Express or Kestrel).

🗂 Project structure
bash
Copy code
MunicipalServicesMvc/
├─ Controllers/
│  ├─ HomeController.cs         # Landing page + recent tickets
│  └─ IssuesController.cs       # Create ticket, save uploads, details view
├─ Models/
│  ├─ Issue.cs                  # Domain model (uses AttachmentList)
│  └─ IssueCreateViewModel.cs   # Form binding model
├─ Services/
│  ├─ IssueList.cs              # CUSTOM: singly linked list of Issue
│  ├─ AttachmentList.cs         # CUSTOM: singly linked list of string paths
│  └─ IssueStore.cs             # In-memory store + manual file persistence
├─ Views/
│  ├─ Home/Index.cshtml         # Landing page (cards, coming soon)
│  └─ Issues/
│     ├─ Create.cshtml          # Form + progress bar (engagement)
│     └─ Details.cshtml         # Confirmation + WhatsApp share button
├─ wwwroot/uploads/             # Uploaded files grouped per {IssueId}/
├─ App_Data/issues.txt          # Line-based persistence (auto-created)
└─ Program.cs                   # Service registration, static files, routes
📱 WhatsApp-only engagement
After submitting a report, the Details page shows Share via WhatsApp Web.

Opens wa.me with a prefilled, human-readable message citizens can forward to ward/street groups.

This submission intentionally implements WhatsApp only (no SMS/USSD/“data-free” web).

🧩 Custom data structures (no built-ins)
To satisfy the “build your own data structure” requirement, the data path uses custom linked lists only:

IssueList

Node: class Node { public Issue Value; public Node? Next; }

Operations:
Add(Issue) O(1) append via tail pointer;
FindById(int) linear scan;
Last(int n) streams the last n items by skipping without arrays/LINQ;
custom enumerator that walks nodes.

AttachmentList

Node: class Node { public string Value; public Node? Next; }

Operations: Add(string) (append O(1)); custom enumerator for iteration.

No List<T>, arrays, LINQ, Queue<T>, HashSet<T>, etc. are used for storage/manipulation.
MVC’s IFormFileCollection is used only at the input edge (binding), not as storage.

💾 Persistence (manual, line-based)
File: App_Data/issues.txt (auto-created on first write)
Encoding: Uri.EscapeDataString to avoid delimiter collisions.

Format

pgsql
Copy code
ISSUE|{Id}|{CreatedAtISO8601}|{LocationEnc}|{CategoryEnc}|{DescriptionEnc}|{StatusEnc}
ATTACH|{PathEnc}
ATTACH|{PathEnc}
END
Example

perl
Copy code
ISSUE|1|2025-09-04T12:00:00.0000000Z|Bosman%20St|Roads|Pothole%20near%20corner|Received
ATTACH|/uploads/1/photo.jpg
END
On load: the app reads line-by-line, reconstructs each Issue, and populates AttachmentList node-by-node (no conversion to generic lists/arrays).

🖼️ Using the app (demo flow)
Open the landing page → see three tasks (two disabled).

Click Report an Issue.

Fill Location, Category, Description (watch the progress bar).

(Optional) Attach an image/document.

Submit → view ticket page (ID, details, attachments).

Click Share via WhatsApp Web → show prefilled message.

Show wwwroot/uploads/{IssueId}/… and App_Data/issues.txt.

✅ Marking checklist (mapping to brief)
Startup menu with three tasks; two disabled ✔️

Report Issues page: Location, Category, Description ✔️

Media attachments via file input (multiple) ✔️

Submit button; confirmation page with ticket details ✔️

Engagement feature: progress bar + helper micro-copy ✔️

Navigation: Back to Home ✔️

Consistency & clarity: Bootstrap layout, concise labels ✔️

Responsiveness: Bootstrap grid ✔️

Event handling: validation, uploads, routing ✔️

Data handling: custom structures (IssueList, AttachmentList) ✔️

Persistence: manual line-based file (no built-ins for serialization) ✔️

README: this file ✔️

WhatsApp-only strategy implemented ✔️

⚙️ Configuration notes
Uploads folder: created on startup at wwwroot/uploads/

Persistence file: created on first write at App_Data/issues.txt

HTTPS: dev certificate enabled by default
If prompted, trust the cert:

bash
Copy code
dotnet dev-certs https --trust
🧩 Troubleshooting
Uploads not visible in browser
Ensure app.UseStaticFiles(); is in Program.cs and files exist under wwwroot/uploads/{IssueId}/.

App_Data/issues.txt missing
Submit at least one ticket (file is created on append).

HTTPS dev cert warnings
Run dotnet dev-certs https --trust and restart the app/IDE.

File I/O denied
Make sure the process has write permission to the repository directory.

📹 Demo video
YouTube: https://youtu.be/Chgozli0Q2o

Suggested recording flow: follow “Using the app (demo flow)” above.

📚 References (Anglia Harvard)
Microsoft (no date) Build your first ASP.NET Core MVC app with controllers and views. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-9.0&tabs=visual-studio (Accessed: 8 September 2025).

GeeksforGeeks (no date) Singly Linked List — Tutorial. Available at: https://www.geeksforgeeks.org/dsa/singly-linked-list-tutorial/ (Accessed: 8 September 2025).

Govender, D.S. (no date) Programming 3B (PROG7312) — Learning Material: Linked List. [PDF lecture notes]. (Accessed: 8 September 2025).

OpenAI (no date) ChatGPT [AI language model]. Available at: https://openai.com/chatgpt (Accessed: 8 September 2025).

QuillBot (no date) QuillBot Paraphraser [AI writing assistant]. Available at: https://quillbot.com/ (Accessed: 8 September 2025).
