﻿using Taskregister.Server.Todos.Contstants;

namespace Taskregister.Server.Todos.Controller.Dto
{
    public class UpdateTodoDto
    {
        public TaskType Type { get; set; }
        public Priority Priority { get; set; }
        //public State State { get; set; }
        public string? Description { get; set; }
    }
}