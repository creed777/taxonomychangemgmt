#Purpose 
The purpose of the Taxonomy Change Management Application is to replace the existing PowerApps: Taxonomy Change Management and Queue Change Management.  These functions will be combined into a single application.

#Technology Requirements
1. Business Context

The system exists in the Human Resources subject area.  The system is designed to allow Human Resources and Human Resources Data Management to manage the Taxonomy structure and CRM email queues, and communicate changes to dependent systems, as well as keeping a record of changes to the system.
The system sends notifications and reports to the SMTP server for distribution to the appropriate individuals or teams.
A list of changes is used by the CRM Team to manually update their system with any changes.

2. Performance Standards
- Users:  The system will support ~200 users requests per month during each change window.
- Concurrent Users: The system will support ~10-20 concurrent users during each change window.
- Responsiveness:  Web pages will completely render to the user within 10 seconds of a triggering event.

3. Technology
- Front End
    - Blazor was chosen due to the increasing acceptance and is easily supported.
    - Syncfusion will provide UI controls that are highly customizable and reduce development time.

- Middle Tier
    - C# is the preferred web development programming language and is easily supported.
    - Azure Active Directory will serve as authentication for the system.  This fulfils the requirement for SSO and allows us to use the current AAD groups.
    - SMTP will be used to send notifications to users or groups for Change Request status changes and other communications.
    - In a future release, the email change report sent to the CRM team should be replaced by an integration with Dynamics 365 Queue Management.

- Data Tier
    - Azure SQL will be used to store the data for the system.  A full SQL Server instance is not required and there is no team to manage it.
    - Date Time entries will be stored as UTC in the database.  This eliminates any confusion on where servers are located and creating disparate time entries.

- Dev Ops
    - The source code will be managed in Azure DevOps
    - A CI/CD pattern will be used to manage Development and QA environments using Azure DevOps pipelines.  At this time the production pipeline will be triggered manually.
    - The system will be deployed to an Azure App Service.  The system does not need a full web service deployment and there is no team to manage it.  App service also allows the flexibility to expand horizontally as required during peak usage.

4. Change Management Data 
- ERD
    - The ERD shows the current understanding of the change data and its relationships.
