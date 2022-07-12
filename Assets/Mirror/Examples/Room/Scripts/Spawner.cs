using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    internal class Spawner: MonoBehaviour
    {
        internal static void InitialSpawn()
        {
            if (!NetworkServer.active) return;

            for (int i = 0; i < 10; i++)
                SpawnReward();
        }

        internal static void SpawnReward()
        {
            if (!NetworkServer.active) return;

            NetworkServer.Spawn(Instantiate(((NetworkRoomManagerExt)NetworkManager.singleton).gameManager, Vector3.zero, Quaternion.identity));
        }
    }
}
