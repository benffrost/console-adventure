﻿using System;
using CastleGrimtol.Project;

namespace CastleGrimtol
{
  public class Program
  {
    public static void Main(string[] args)
    {
      GameService Instance = new GameService();

      Instance.StartGame();
    }
  }
}
