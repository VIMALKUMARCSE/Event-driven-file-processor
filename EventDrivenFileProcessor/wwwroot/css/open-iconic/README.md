Event-Driven File Processor

A Blazor Server web application that lets authenticated users upload and delete files via AWS S3, with an event-driven backend on AWS that processes files, tracks their status in DynamoDB, sends notifications, and is fully monitored through CloudWatch.

Overview

Users log in through AWS Cognito and interact with a Blazor Server dashboard to upload and manage files. File uploads land in S3, which triggers an event-driven backend pipeline (Lambda, SNS, SQS) that processes the file, updates its status in DynamoDB, and sends notifications. A REST API built with API Gateway + Lambda exposes POST and DELETE endpoints for file operations. CloudWatch provides logging and monitoring across the pipeline.

Architecture
Browser (Blazor Server UI)
        |
        v
  Blazor Server App -----------> Amazon Cognito   (authentication)
        |
        v
  Amazon API Gateway
     POST /files    -----> AWS Lambda -----> Amazon S3       (upload)
     DELETE /files  -----> AWS Lambda -----> Amazon S3       (delete)
        |
        v (S3 event trigger on upload)
  AWS Lambda (file processing)
        |
        +--> Amazon SNS         (notifications)
        +--> Amazon SQS         (queued processing)
        +--> Amazon DynamoDB    (file history / status)
        |
        v
  Amazon CloudWatch (logs, metrics, monitoring across all Lambda functions)
AWS Services Used
Amazon Cognito — user authentication (login, forgot password)
Amazon API Gateway — REST endpoints (POST to upload, DELETE to remove a file)
AWS Lambda — handles API Gateway requests and processes S3 upload events
Amazon S3 — file storage
Amazon SNS — notifications on file events
Amazon SQS — queues processing tasks between components
Amazon DynamoDB — file history and status tracking
Amazon CloudWatch — logging and monitoring for Lambda executions and API Gateway
IAM — scoped permissions between services (no hard-coded credentials)
Tech Stack

C# / .NET 8, Blazor Server, MudBlazor, AWS SDK for .NET (S3, DynamoDB, CognitoIdentityProvider), AWS Lambda, API Gateway

API Endpoints
Method	Endpoint	Purpose
POST	/files	Upload a new file, triggers processing pipeline
DELETE	/files/{id}	Delete a file from S3 and remove its history record
Project Structure
EventDrivenFileProcessor/
├── Pages/              # Login, ForgotPassword, FileDashboard, FileHistoryDashboard
├── Services/           # S3Service, FileHistoryService, CognitoAuthService, CustomAuthenticationStateProvider
├── Models/             # CognitoSettings, FileHistory, FileRecord
├── Shared/              # Layout, NavMenu
├── lambda/              # (add if you export your Lambda function code)
├── Program.cs
└── appsettings.json     # kept out of source control — see appsettings.Example.json
Authentication & Security
Users authenticate against an AWS Cognito User Pool.
No AWS credentials are hard-coded — services use IAM roles/policies scoped to what each component needs (S3 read/write, DynamoDB read/write, SNS publish, SQS send/receive).
Real config values (bucket name, pool ID, client secret) are excluded from source control; use appsettings.Example.json as the template.
Monitoring

CloudWatch is used to track Lambda invocations, errors, and duration across the processing pipeline, along with API Gateway request logs — giving visibility into the full upload-to-processing flow.

Deployment
Clone the repo.
Copy appsettings.Example.json → appsettings.json and fill in your own AWS values.
Deploy the Lambda functions and API Gateway routes (POST /files, DELETE /files/{id}) separately in AWS, or via IaC if you add one.
Ensure your AWS credentials (IAM role with S3, DynamoDB, SNS, SQS, and CloudWatch permissions) are available to the app.
dotnet restore && dotnet run
Lessons Learned
Designing an event-driven pipeline where an S3 upload fans out through Lambda, SNS, and SQS rather than doing everything synchronously in the web app.
Structuring Blazor Server auth state around a custom AuthenticationStateProvider backed by Cognito.
Keeping AWS secrets out of source control from day one rather than retrofitting it.
Using CloudWatch to trace a request end-to-end across API Gateway and multiple Lambda fu

Test push - checking sync

Event-Driven File Processor | GitHub : https://github.com/VIMALKUMARCSE/Event-driven-file-processor
C#, .NET 8, Blazor Server, MudBlazor, AWS Lambda, API Gateway, S3, Cognito, DynamoDB, SNS, SQS, CloudWatch, IAM

Developed a Blazor Server web app with AWS Cognito authentication, implementing secure login and password-recovery flows.
Designed and deployed a REST API using Amazon API Gateway and AWS Lambda to handle file upload and delete operations.
Architected an event-driven processing pipeline where S3 upload events trigger Lambda functions, using SNS for notifications and SQS for decoupled, queued processing.
Built a DynamoDB-backed data layer to track file status and history, supporting efficient lookups and real-time updates.
Implemented monitoring with Amazon CloudWatch across Lambda and API Gateway, and enforced IAM least-privilege access instead of hard-coded credentials.