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
          case "drop":
            Drop(option);
            break;
          case "inventory":
            Inventory();
            break;
          case "reset":
            Reset();
            break;
          case "help":
            Help();
            break;
          case "talk":
            if (CurrentRoom.Name == "Auditorium" && CurrentPlayer.HasFishInEar)
            {
              CurrentRoom.CallBack(CurrentPlayer, CurrentRoom);
            }
            else Console.WriteLine("Blah blah blah");
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
      Console.WriteLine("You can: \nTake\nUse\nDrop\nGo\nTalk\nLook\nInventory\nReset\nQuit\n");
    }

    public void Inventory()
    {
      Console.WriteLine("You have:");
      foreach (Item thing in CurrentPlayer.Inventory)
      {
        Console.WriteLine(thing.Name + " - " + thing.Description);
      }
    }

    public void Drop(string itemName)
    {
      Item thing = CurrentPlayer.Inventory.Find(el => el.Name == itemName);

      if (thing != null)
      {
        CurrentPlayer.Inventory.Remove(thing);
        CurrentRoom.Items.Add(thing);

        Console.WriteLine("You dropped the " + itemName);
      }
    }


    public void Look()
    {
      Console.WriteLine(CurrentRoom.Description);
      Console.WriteLine("In this room you see:");
      foreach (Item thing in CurrentRoom.Items)
      {
        Console.WriteLine("A " + thing.Name);
      }
    }

    public void Quit()
    {
      Running = false;
      Console.WriteLine("Goodbye!");
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
      Room start = new Room("Sleep Chamber", "This is the dimly lit room where you woke up.  There is a door to the north.");
      Room hallway = new Room("Hallway", "A boring hallway.  Exits to the east, west, and south.");
      Room tank = new Room("Fish Tank", "There's a fish tank in this room. Exits to the east.");
      Room auditorium = new Room("Auditorium", "There's a grotesque alien here blocking the door to the north, and a door to the east.");
      Room victory = new Room("Space Buffet", "You made it to the space buffet! Good job!");

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
            auditorium.Description = "This is an auditorium with exits to the north and west.";
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

      Console.WriteLine("You wake up from hypersleep.");
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