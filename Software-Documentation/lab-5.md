# Laboratory Work 5: Unit Testing
**Messenger API - ASP.NET Core Implementation**

This document outlines the test cases used to verify the integrity of the Messenger API. The testing suite uses `xUnit` and the Entity Framework Core `InMemory` database provider to ensure strict isolation of functions without affecting the production SQLite database.

---

## 1. Business Logic Testing Checklist
This checklist covers the core domain operations, ensuring relational integrity, correct HTTP responses, and proper handling of invalid inputs.

| ID | Module / Controller | Scenario (Test Case) | Type | Expected Result | Status |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **TC-1.1** | `UsersController` | Create a user with a valid, populated name string. | Positive | Returns `201 Created`. Database count increases by 1. | Passed |
| **TC-1.2** | `UsersController` | Attempt to create a user with an empty or whitespace-only name. | Negative | Returns `400 BadRequest`. Database remains unchanged. | Passed |
| **TC-1.3** | `ConversationsController` | Create a conversation without explicitly providing a `Type`. | Positive | Returns `201 Created`. Defaults to `type: "direct"`. | Passed |
| **TC-1.4** | `ConversationsController` | Retrieve message history containing a mix of visible and hidden messages. | Positive | Returns `200 OK`. Result excludes messages where `IsHidden = true` and orders by `CreatedAt`. | Passed |
| **TC-1.5** | `MessagesController` | Send a message with valid `ConversationId`, `SenderId`, and `Text`. | Positive | Returns `200 OK` and a valid `MessageId`. Database count increases by 1. | Passed |
| **TC-1.6** | `MessagesController` | Attempt to send a message using non-existent User or Conversation IDs. | Negative | Returns `404 NotFound`. Database remains unchanged. | Passed |
| **TC-1.7** | `MessagesController` | Delete a message using a valid existing `MessageId`. | Positive | Returns `204 NoContent`. Message is physically removed from the database. | Passed |
| **TC-1.8** | `ModerationController` | Resolve a report with the `HIDE` action. | Positive | Returns `200 OK`. Original message is updated with `IsHidden = true` and the moderation reason is saved. | Passed |
| **TC-1.9** | `ModerationController` | Resolve a report with the `DELETE` action. | Positive | Returns `200 OK`. Report is marked resolved and the original message is physically removed. | Passed |

---

## 2. Auxiliary Tasks Testing Checklist
This checklist covers non-database utility functions, specifically verifying the file system operations used to export chat histories.

| ID | Module / Service | Scenario (Test Case) | Type | Expected Result | Status |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **TC-2.1** | `ChatExportService` | Export a valid, populated list of `Message` objects to a valid temporary file path. | Positive | Returns `true`. A text file is created on the disk containing the formatted chat history. | Passed |
| **TC-2.2** | `ChatExportService` | Attempt to export an empty list of messages. | Negative | Returns `false`. Fails gracefully without throwing exceptions; no file is created. | Passed |