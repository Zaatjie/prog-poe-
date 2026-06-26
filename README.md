# CyberSecurity Awareness Chatbot

## Overview

The **CyberSecurity Awareness Chatbot** is a Windows Forms application developed in **C#** that educates users about cybersecurity while providing interactive features such as task management, a cybersecurity quiz, natural language processing (NLP), and activity tracking. The application also uses a **MySQL database** to store user tasks.

---

## Features

### 💬 Chatbot

* Interactive chatbot interface.
* Responds to common cybersecurity questions.
* Uses keyword-based Natural Language Processing (NLP) to detect user intent.
* Supports conversational commands such as:

  * Password security
  * Phishing
  * Safe browsing
  * Two-Factor Authentication (2FA)
  * Malware
  * VPNs
  * Help menu

---

### 📋 Task Assistant

* Create cybersecurity-related tasks.
* Automatically generates descriptions for common security tasks.
* Optional reminder feature.
* Stores tasks in a MySQL database.
* View, complete, and delete tasks.

---

### 🎮 Cybersecurity Quiz

* Randomises quiz questions each time the quiz starts.
* Multiple-choice and True/False questions.
* Immediate feedback after every answer.
* Displays explanations for correct answers.
* Calculates and displays the final score.

---

### 📜 Activity Log

* Records important chatbot actions such as:

  * Tasks created
  * Tasks completed
  * Quiz started and completed
  * Security advice requested
* Displays the 10 most recent activities.

---

## Technologies Used

* C#
* Windows Forms (.NET)
* MySQL
* MySql.Data Connector
* LINQ
* Object-Oriented Programming (OOP)

---

## Database

The application automatically creates a **tasks** table containing:

* ID
* Title
* Description
* Reminder
* Completion Status
* Date Created

---

## Project Structure

* **Program** – Starts the application.
* **Database** – Handles all MySQL database operations.
* **TaskItem** – Represents a task object.
* **QuizQuestion** – Represents quiz questions.
* **QuizData** – Stores cybersecurity quiz questions.
* **Nlp** – Detects user intent using keyword matching.
* **MainForm** – Contains the graphical user interface and chatbot functionality.

---

## Key Programming Concepts

* Object-Oriented Programming
* Collections (Lists and Dictionaries)
* Exception Handling
* Database Connectivity
* Event-Driven Programming
* Natural Language Processing (Keyword Matching)
* CRUD Operations
* Windows Forms GUI Development

---

## Example Commands

* password
* phishing
* safe browsing
* malware
* vpn
* 2FA
* add task Enable two-factor authentication
* view tasks
* start quiz
* show activity log
* help

---

## Installation

1. Install Visual Studio with .NET Windows Forms support.
2. Install MySQL Server.
3. Create a database named **cyberchatbot**.
4. Install the **MySql.Data** NuGet package.
5. Update the database connection string if required.
6. Build and run the application.

---

## Future Improvements

* Voice recognition
* AI-powered chatbot responses
* User login system
* Reminder notifications
* Password strength checker
* Export activity logs
* Dashboard with cybersecurity statistics

---

## Author

**CyberSecurity Awareness Chatbot**

A Windows Forms application developed to promote cybersecurity awareness through interactive conversations, task management, quizzes, and activity tracking.
