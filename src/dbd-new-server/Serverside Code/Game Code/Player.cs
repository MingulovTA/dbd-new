using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample
{
    public class Player : BasePlayer 
    {
        public string Name = "";
        public int Score = 0;
        public int TeamId = 0;

        public float PosX = 0;
        public float PosY = 0;
        public float PosZ = 0;
    }
}