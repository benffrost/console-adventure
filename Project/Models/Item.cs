using System;
using System.Collections.Generic;
using CastleGrimtol.Project.Interfaces;

namespace CastleGrimtol.Project.Models
{
  public class Item : IItem
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public Action<Player, Room> Use { get; set; }

    public Item(string name, string description, Action<Player, Room> use)
    {
      Name = name;
      Description = description;
      Use = use;
    }
  }
}