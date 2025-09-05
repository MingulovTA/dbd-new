using System;
using System.Linq;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample 
{
	[RoomType("UnityMushrooms")]
	public class GameCode : Game<Player>
	{
		private Time _time;
		private GameStateId _gameStateId;
		public override void GameStarted() 
		{
			if (_time == null)
			{
				_time = new Time();
				AddTimer(_time.TickSecond, 1000);
			}
			Console.WriteLine("Game is started: " + RoomId);
			RestartGame();
			
		}

		private void RestartGame() 
		{
			Console.WriteLine("RestartGame");
			foreach(Player pl in Players) 
			{
				pl.Score = 0;
				pl.TeamId = 0;
			}
			Broadcast("SvRestartGame");
			_gameStateId = GameStateId.WarmUp;
			_time.Invoke(KillFirst,5);

		}

		private void KillFirst()
		{
			Console.WriteLine("KillFirst");
			_gameStateId = GameStateId.Gameplay;
			Random random = new Random();
			int randomIndex = random.Next(0, PlayerCount-1);
			var firstKiller = Players.ToList()[randomIndex];
			firstKiller.TeamId = 1;
			Broadcast("SvKill", "Server", firstKiller.ConnectUserId);
		}


		public override void GameClosed() 
		{
			Console.WriteLine("RoomId: " + RoomId);
		}

		public override void UserJoined(Player player) 
		{
			foreach(Player pl in Players) 
			{
				if(pl.ConnectUserId != player.ConnectUserId)
				{
					pl.Send("SvUserJoined", player.ConnectUserId, player.PosX,player.PosY,player.PosZ,player.TeamId);
					player.Send("SvUserJoined", pl.ConnectUserId, pl.PosX, pl.PosY, pl.PosZ,pl.TeamId);
				}
			}
			player.Send("SvUserJoined", player.ConnectUserId, player.PosX,player.PosY,player.PosZ,player.TeamId);
			CheckForEndOfRound();
		}

		public override void UserLeft(Player player) 
		{
			Broadcast("SvUserLeft", player.ConnectUserId);
			CheckForEndOfRound();
		}

		public override void GotMessage(Player player, Message message) 
		{
			switch(message.Type) 
			{
				case "ClMove":
					player.PosX = message.GetFloat(0);
					player.PosY = message.GetFloat(1);
					player.PosZ = message.GetFloat(2);
					Broadcast("SvMove", player.ConnectUserId, player.PosX, player.PosY, player.PosZ);
					break;
				case "ClTurnY":
					Broadcast("SvTurnY", player.ConnectUserId, message.GetFloat(0));
					break;
				case "ClRevive":
					var reviverId = message.GetString(0);
					var reviver = Players.FirstOrDefault(p => p.ConnectUserId == reviverId);

					if (reviver != null && reviver.TeamId == 1)
					{
						reviver.TeamId = 0;
						Broadcast("SvRevive", reviverId);
						CheckForEndOfRound();
					}
					break;
				case "ClKill":
					var targetId = message.GetString(0);
					var targetPlayer = Players.FirstOrDefault(p => p.ConnectUserId == targetId);

					if (targetPlayer != null /*&& player.TeamId == 1*/ && targetPlayer.TeamId == 0)
					{
						targetPlayer.TeamId = 1;
						Broadcast("SvKill", player.ConnectUserId, targetId);
						player.Score++;
						Broadcast("SvScoreChanged", player.ConnectUserId, player.Score);
						CheckForEndOfRound();
					}
					break;
				case "ClKilledBy":
					var killerID = message.GetString(0);
					var killerPlayer = Players.FirstOrDefault(p => p.ConnectUserId == killerID);

					if (killerPlayer != null /*&& player.TeamId == 0*/ && killerPlayer.TeamId == 1)
					{
						player.TeamId = 1;
						Broadcast("SvKill", killerID, player.ConnectUserId);
						killerPlayer.Score++;
						Broadcast("SvScoreChanged", killerID, killerPlayer.Score);
						CheckForEndOfRound();
					}
					break;

				case "Chat":
					Console.WriteLine("player.ConnectUserId: {0}", message.GetString(0));
					foreach(Player pl in Players) {
						if(pl.ConnectUserId != player.ConnectUserId) 
						{
							pl.Send("Chat", player.ConnectUserId, message.GetString(0));
						}
					}
					break;
			}
		}

		private void CheckForEndOfRound()
		{
			if (_gameStateId==GameStateId.Gameplay&&Players.All(p => p.TeamId == 1))
			{
				_gameStateId = GameStateId.Complete;
				_time.Invoke(RestartGame,5);
			}
		}
	}
}