# Online Greadebook System

## General Description

This project is a comprehensive web application designed to manage academic records and activities in a university setting. The system supports multiple user roles, each with distinct functionalities, ensuring efficient management and access to academic data.

## Requirements and Features

1. **User Roles:**
   - Student
   - Professor
   - Secretary
   - Moderator

2. **Student Features:**
    - View Courses and Grades: Students can see a list of their courses, grades, and GPA for each academic year.
    Sort Courses: Students can sort courses alphabetically or by grade.
    - Request Certificates: Students can request student certificates, which are automatically generated in PDF format if they are enrolled in a study program for the current year.
    - Notifications: Students receive alerts when they are added to new courses or when new grades are posted.

3. **Professor Features:**
    - Add Grades: Professors can add grades for students in their courses.
    - Receive Messages: Professors can receive messages from secretaries.

4. **Secretarie Features:**
    - Access Gradebooks: Secretaries have access to all course and group gradebooks.
    - Export to PDF: Secretaries can export gradebooks as PDF files.
    - Search Courses: Courses can be searched by name or professor.
    - Notify Professors: Secretaries can write notification messages to professors, selecting them individually or by department.

5. **Moderator Features:**
    - Manage Courses: Moderators can create, modify, or delete courses.
    - Assign Students and Professors: Moderators can add or remove students and professors from courses.


## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- HTML, CSS, JavaScript
- Bootstrap
- PDF generation tools (itext7)

