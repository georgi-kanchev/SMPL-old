﻿using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class WorldObjectManager
	{
		public static void InitializeObjects()
		{
			var freeTile = new Point(0, 0);
			var playerTiles = new List<Point>();
			foreach (var kvp in World.RawData)
			{
				var pos = new Point(kvp.Key.X, kvp.Key.Y);
				if (World.RawData[pos][3] == World.TilePlayer)
				{
					playerTiles.Add(pos);
					World.RawData[pos][3] = new();
				}
				else if (World.RawData[pos][3] == World.TileBarrier)
				{
					var tile = World.TileBarrier;
					tile.C = new Color();
					World.RawData[pos][3] = tile;
				}
				else if (World.RawData[pos][3] == new Point(0, 0))
					freeTile = pos;
			}

			var randPoint = playerTiles.Count > 0 ?
				playerTiles[(int)Probability.Randomize(new(0, playerTiles.Count - 1))] : freeTile;

			var player = Assets.ValuesAreLoaded(nameof(Player)) == false
						? new Player(nameof(Player), new Object.CreationDetails()
						{
							Name = nameof(Player),
							Position = randPoint,
							Height = 3,
							TileIndexes = new Point[] { new(25, 0) }
						})
						: Text.FromJSON<Player>(Assets.GetValue(nameof(Player)));
			World.CameraPosition = player.Position;
			player.PreviousPosition = player.Position;
			World.IsShowingRoofs = World.TileHasRoof(player.Position) == false;

			LoadAll<Chest>(); LoadAll<ItemPile>(); LoadAll<Bag>(); LoadAll<Quiver>(); LoadAll<Key>(); LoadAll<Map>();

			void LoadAll<T>()
			{
				if (Assets.ValuesAreLoaded(typeof(T).Name))
					Text.FromJSON<T[]>(Assets.GetValue(typeof(T).Name));
			}
		}

		public static void OnAdvanceTime()
		{
			Door.TryToCreate();
			Boat.TryToCreate();
			Chest.TryToCreate();

			var player = (Player)Object.PickByUniqueID(nameof(Player));
			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
					if (kvp.Value[i] is IDeletableWhenFar && Point.Distance(player.Position, kvp.Value[i].Position) > 5)
						objsToDestroy.Add(kvp.Value[i]);
			for (int i = 0; i < objsToDestroy.Count; i++)
			{
				World.RawData[objsToDestroy[i].Position][objsToDestroy[i].Height] = objsToDestroy[i].TileIndexes;
				objsToDestroy[i].Destroy();
			}
		}
	}
}