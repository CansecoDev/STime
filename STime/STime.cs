using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using TerrariaApi.Server;

using TShockAPI;

namespace STime
{
    [ApiVersion(1, 20)]
    public class STime : TerrariaPlugin
    {
        public override string Name
        {
            get { return "Silent Time"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        public override string Author
        {
            get { return "Canseco"; }
        }

        public override string Description
        {
            get { return "Time changer based on /time, but silent."; }
        }

        public STime(Main game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("stime.allow", SilentTime, "stime"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { }
            base.Dispose(disposing);
        }

        private void SilentTime(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                double time = Main.time / 3600.0;
                time += 4.5;
                if (!Main.dayTime)
                    time += 15.0;
                time = time % 24.0;
                args.Player.SendInfoMessage("The current time is {0}:{1:D2}.", (int)Math.Floor(time), (int)Math.Round((time % 1.0) * 60.0));
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
                case "day":
                    TSPlayer.Server.SetTime(true, 0.0);
                    break;
                case "night":
                    TSPlayer.Server.SetTime(false, 0.0);
                    break;
                case "noon":
                    TSPlayer.Server.SetTime(true, 27000.0);
                    break;
                case "midnight":
                    TSPlayer.Server.SetTime(false, 16200.0);
                    break;
                default:
                    string[] array = args.Parameters[0].Split(':');
                    if (array.Length != 2)
                    {
                        args.Player.SendErrorMessage("Invalid time string! Proper format: hh:mm, in 24-hour time.");
                        return;
                    }

                    int hours;
                    int minutes;
                    if (!int.TryParse(array[0], out hours) || hours < 0 || hours > 23
                        || !int.TryParse(array[1], out minutes) || minutes < 0 || minutes > 59)
                    {
                        args.Player.SendErrorMessage("Invalid time string! Proper format: hh:mm, in 24-hour time.");
                        return;
                    }

                    decimal time = hours + (minutes / 60.0m);
                    time -= 4.50m;
                    if (time < 0.00m)
                        time += 24.00m;

                    if (time >= 15.00m)
                    {
                        TSPlayer.Server.SetTime(false, (double)((time - 15.00m) * 3600.0m));
                    }
                    else
                    {
                        TSPlayer.Server.SetTime(true, (double)(time * 3600.0m));
                    }
                    break;
            }
        }
    }
}