﻿using System.IO;
using GabberPCL.Models;
using SQLite;

namespace GabberPCL
{
    // Singleton and persistent connection to database to avoid expensive
    // opening/closing operations on the database file
    public static class Session
    {
        // TODO: this should be set from LoginViewController
        public static User ActiveUser = new User
        {
            Id = 1,
            Name = "You",
            Email = "TODO_AFTER_LOGIN",
            JWToken = "blahblah",
            Selected = true,
            IsActive = false
        };

        static SQLiteConnection _connection;
        // Used to access platform specific implementations
        public static Interfaces.IPrivatePath PrivatePath;

        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(Path.Combine(PrivatePath.PrivatePath(), "gabber.db3"));
                    _connection.CreateTable<User>();
                    _connection.CreateTable<Project>();
                    _connection.CreateTable<Prompt>();
                    _connection.CreateTable<InterviewSession>();
                    _connection.CreateTable<InterviewParticipant>();
                    _connection.CreateTable<InterviewPrompt>();
                }
                return _connection;
            }
        }
    }
}