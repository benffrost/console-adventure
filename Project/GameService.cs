using System;
using System.Collections.Generic;
using CastleGrimtol.Project.Interfaces;
using CastleGrimtol.Project.Models;

namespace CastleGrimtol.Project
{
  public class GameService : IGameService
  {
    public IRoom CurrentRoom { get; set; }
    public Player CurrentPlayer { get; set; }
    public bool Running { get; set; } = true;

    public void GetUserInput()
    {
      bool repeat = false;

      do
      {
        System.Console.Write("What do you want to do: ");

        string[] input = Console.ReadLine().ToLower().Split(' ');
        string command = input[0];
        string option = "";

        if (input.Length > 1)
        {
          option += input[1];
          // if (input.Length > 2)
          // {
          //   for (int i = 2; i < input.Length; i++)
          //   {
          //     option += " " + input[i];
          //   }
          // }
        }

        switch (command)
        {
          case "take":
            TakeItem(option);
            break;
          case "put":
          case "use":
            UseItem(option);
            break;
          case "go":
          case "head":
          case "walk":
            Go(option);
            break;
          case "north":
          case "n":
            Go("north");
            break;
          case "south":
          case "s":
            Go("south");
            break;
          case "west":
          case "w":
            Go("west");
            break;
          case "east":
          case "e":
            Go("east");
            break;
          case "look":
            Look();
            break;
          case "inventory":
            Inventory();
            break;
          case "reset":
            Reset();
            break;
          case "quit":
            Quit();
            break;
          default:
            System.Console.WriteLine("Unknown Command");
            repeat = true;
            break;
        }
      } while (repeat == true);

    }

    public void Go(string direction)
    {
      CurrentRoom = CurrentRoom.Go(direction);
      CurrentRoom.CallBack(CurrentPlayer, CurrentRoom);
    }

    public void Help()
    {
      Console.WriteLine("No help!  Figure it out!");
    }

    public void Inventory()
    {
      foreach (Item thing in CurrentPlayer.Inventory)
      {
        Console.WriteLine(thing.Name + " - " + thing.Description);
      }
    }

    public void Look()
    {
      Console.WriteLine(CurrentRoom.Description);
    }

    public void Quit()
    {
      Console.WriteLine("Goodbye!");
      Running = false;
    }

    public void Reset()
    {
      Setup();
    }

    public void Setup()
    {
      //wild items
      Item fish = new Item("fish", "a small yellow fish", (Player p, Room r) => p.HasFishInEar = true);

      //rooms
      Room start = new Room("sleep chamber", "This is the dimly lit room where you woke up.  There is a door to the north.");
      Room hallway = new Room("hallway", "A boring hallway.  Exits to the east, west, and south.");
      Room tank = new Room("fish tank", "There's a fish tank in this room. Exits to the east.");
      Room auditorium = new Room("auditorium", "There's a grotesque alien here blocking the door to the north, and a door to the east.");
      Room victory = new Room("buffet", "You made it to the space buffet! Good job!");

      //room links
      start.Exits.Add("north", hallway);

      hallway.Exits.Add("west", tank);
      hallway.Exits.Add("east", auditorium);
      hallway.Exits.Add("south", start);

      tank.Exits.Add("east", hallway);

      auditorium.Exits.Add("west", hallway);

      //items in rooms
      tank.Items.Add(fish);

      //room callbacks

      auditorium.CallBack = (Player p, IRoom r) =>
      {
        Console.WriteLine("There's a large, grotesque alien blocking the north exit of the room.");

        if (p.HasFishInEar)
        {
          char input;
          do
          {
            Console.WriteLine("The alien asks, \"Would you like to hear my poetry?\"");
            input = char.ToLower((Console.ReadKey().KeyChar));
          } while (input != 'y' && input != 'n');

          if (input == 'Y')
          {
            Running = false;
            Console.WriteLine("You soon die of boredom.");
          }
          else
          {
            Console.WriteLine("Well, you've hurt his feelings now.  He sulks off.");
            auditorium.CallBack = (Player _p, IRoom _r) => { };
            auditorium.Exits.Add("north", victory);


          }

        }
        else Console.WriteLine("The alien speaks to you energetically in a language you do not understand.");
      };

      victory.CallBack = (Player p, IRoom r) =>
      {
        Console.WriteLine("You win!");
        Running = false;
      };

      //player state
      CurrentRoom = start;
      Console.WriteLine("Enter your name:");
      CurrentPlayer = new Player(Console.ReadLine());

      Console.Clear();

      Console.WriteLine("INTRO TEXT");
    }

    public void StartGame()
    {
      Setup();

      Console.WriteLine(CurrentRoom.Name);
      // CurrentRoom.PrintDirections();
      while (Running)
      {
        GetUserInput();
      }
    }

    public void TakeItem(string itemName)
    {
      Item thing = CurrentRoom.Items.Find(el => el.Name == itemName);
      if (thing != null)
      {
        CurrentPlayer.Inventory.Add(thing);
        CurrentRoom.Items.Remove(thing);

        Console.WriteLine("You picked up a " + itemName);
      }
    }

    public void UseItem(string itemName)
    {
      Item thing = CurrentPlayer.Inventory.Find(el => el.Name == itemName);

      if (thing != null)
      {
        thing.Use(CurrentPlayer, (Room)CurrentRoom);
      }
      else Console.WriteLine("You don't have a " + itemName + " to use!");
    }
  }
}