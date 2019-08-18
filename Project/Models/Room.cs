using System;
using System.Collections.Generic;
using CastleGrimtol.Project.Interfaces;

namespace CastleGrimtol.Project.Models
{
  public class Room : IRoom
  {

    public string Name { get; set; }
    public string Description { get; set; }
    public List<Item> Items { get; set; }
    public Dictionary<string, IRoom> Exits { get; set; }
    public Action<Player, IRoom> CallBack { get; set; }

    public void PrintDirections()
    {
      foreach (var location in Exits)
      {
        System.Console.Write(location.Key + ", ");
      }
      System.Console.WriteLine("");
    }

    public Room(string name, string description)
    {
      Name = name;
      Description = description;
      Items = new List<Item>();
      Exits = new Dictionary<string, IRoom>();
      CallBack = (Player p, IRoom r) => { };
    }

    public IRoom Go(string direction)
    {

      if (Exits.ContainsKey(direction))
      {
        Console.Clear();
        IRoom nextRoom = Exits[direction];
        Console.WriteLine("You walk into the " + nextRoom.Name);
        return nextRoom;
      }
      Console.WriteLine("Whoops! You clumsily walk into a wall.");
      return this;


    }
  }
}