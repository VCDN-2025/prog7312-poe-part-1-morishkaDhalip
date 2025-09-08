MUNICIPAL SERVICES MVC APPLICATION

Prerequisites:
- .NET 8 SDK (dotnet --version → 8.x)
- Windows/macOS/Linux
- (Optional) Visual Studio 2022 / VS Code / Rider

Run from the CLI:
# from the repository root
dotnet restore
dotnet build
dotnet run


Open the printed URL (e.g., https://localhost:5xxx).

Run in Visual Studio:
- Open the .csproj or solution.
- Set build config to Debug.
- Press F5 (IIS Express or Kestrel).

Project structure:
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
├─ wwwroot/uploads/             # Uploaded files (grouped per {IssueId}/)
├─ App_Data/issues.txt          # Line-based persistence (auto-created)
└─ Program.cs                   # Service registration, static files, routes

 WhatsApp-only engagement (no SMS/USSD/data-free web):
- After submission, Details shows “Share via WhatsApp Web”.
- The link opens wa.me with a prefilled, compact, human-readable message:
  Ticket: {Id} • Category: {Category} • Location: {Location} • Submitted: {YYYY-MM-DD HH:mm}

This submission intentionally implements WhatsApp only as the engagement channel.

🧩 Custom data structures (no built-ins):

To satisfy the “build your own data structure” requirement, the data path uses only custom linked lists:
- IssueList
- Node: class Node { public Issue Value; public Node? Next; }
- Operations: Add(Issue) O(1) append via tail pointer; FindById(int) linear scan; Last(int n) streams the last n items by skipping without arrays/LINQ; custom enumerator that walks nodes.

- AttachmentList
- Node: class Node { public string Value; public Node? Next; }
- Operations: Add(string) (append O(1)); custom enumerator for iteration.
- No List<T>, arrays, LINQ, Queue<T>, HashSet<T>, etc. are used for storage/manipulation.
MVC’s IFormFileCollection is used only at the input edge (model binding), not as storage.

💾 Persistence (manual, line-based):

File: App_Data/issues.txt (auto-created on first run)
Encoding: Fields are URI-encoded (Uri.EscapeDataString) to avoid delimiter collisions.
Format:

ISSUE|{Id}|{CreatedAtISO8601}|{LocationEnc}|{CategoryEnc}|{DescriptionEnc}|{StatusEnc}
ATTACH|{PathEnc}
ATTACH|{PathEnc}
END

Example:

ISSUE|1|2025-09-04T12:00:00.0000000Z|Bosman%20St|Roads|Pothole%20near%20corner|Received
ATTACH|/uploads/1/photo.jpg
END

- On load: The app reads the file line-by-line, reconstructs each Issue, and populates AttachmentList node-by-node. No conversion to generic lists/arrays is performed.

🖼️ Using the app:

- Open the landing page → show three tasks (two disabled).
- Click Report an Issue.
- Fill Location, Category, Description (watch the progress bar update).
- Attach an image or document (optional).
- Click Submit → view the ticket page with ID, details, attachments.
- Click Share via WhatsApp Web → show the prefilled message.
- Show wwwroot/uploads/{IssueId}/… in your file explorer.
- Show App_Data/issues.txt to demonstrate line-based persistence.

🧑‍⚖️ Marking checklist (mapping to brief):

Startup menu with three tasks; two disabled ✔️
Report Issues page: Location (textbox), Category (dropdown), Description (textarea) ✔️
Media attachments via file input (multiple) ✔️
Submit button; clear feedback (confirmation page with ticket details) ✔️
Engagement feature: progress bar + helper micro-copy ✔️
Navigation: Back to Home ✔️
Consistency & clarity: Bootstrap layout, concise labels ✔️
Responsiveness: Bootstrap grid ✔️
Event handling: form validation, file upload, routing ✔️
Data handling: custom structures (IssueList, AttachmentList) ✔️
Persistence: manual line-based file (no built-ins for serialization) ✔️
README: this file ✔️
WhatsApp-only strategy implemented (no SMS/USSD/data-free web) ✔️



🛠️ Configuration notes:

Uploads folder: created on startup at wwwroot/uploads.

Persistence file: created on first write at App_Data/issues.txt.

HTTPS: enabled by default (Kestrel dev cert). If cert prompts appear, trust the dev cert (dotnet dev-certs https --trust).



🧰 Troubleshooting

Uploads not visible in the browser
Ensure app.UseStaticFiles(); is present in Program.cs, and files were saved under wwwroot/uploads/{IssueId}/.

App_Data/issues.txt not created
Submit at least one ticket; the file is created on append.

HTTPS dev cert warnings
Run dotnet dev-certs https --trust and restart the app/IDE.

File I/O denied
Make sure the process has write permission to the repo folder.

📹 Video (YouTube):
link:
https://youtu.be/Chgozli0Q2o

References:

Microsoft (no date) Build your first ASP.NET Core MVC app with controllers and views. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-9.0&tabs=visual-studio
 (Accessed: 7 September 2025).

GeeksforGeeks (no date) Singly Linked List — Tutorial. Available at: https://www.geeksforgeeks.org/dsa/singly-linked-list-tutorial/
 (Accessed: 7 September 2025).

Govender, D.S. (no date) Programming 3B (PROG7312) — Learning Material: Linked List. [PDF lecture notes]. (Accessed: 7 September 2025).

OpenAI ChatGPT [AI language model]. Available at: https://openai.com/chatgpt
 (Accessed: 8 September 2025).

<YOUR LINK>

Suggested flow: follow the Testing tips and Marking demo script above.
