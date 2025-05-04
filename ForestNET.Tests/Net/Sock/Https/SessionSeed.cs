namespace ForestNET.Tests.Net.Sock.Https
{
    internal class SessionSeed : ForestNET.Lib.Net.Https.Dynm.ForestSeed
    {
        public override void PrepareContent()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new InvalidDataException("Seed instance is not available");
            }

            if (this.Seed.SessionData.ContainsKey("4"))
            {
                this.Seed.SessionData["5"] = "five";
            }

            if (this.Seed.SessionData.ContainsKey("3"))
            {
                this.Seed.SessionData["4"] = "four";
            }

            if (this.Seed.SessionData.ContainsKey("2"))
            {
                this.Seed.SessionData["3"] = "three";
            }

            if (this.Seed.SessionData.ContainsKey("1"))
            {
                this.Seed.SessionData["2"] = "two";
            }

            if (this.Seed.SessionData.Count == 0)
            {
                this.Seed.SessionData["1"] = "one";
            }

            this.Seed.Temp.Add("SESSIONLIST", this.Seed.SessionData);
        }
    }
}
