using UnityEngine;

namespace RadFixes
{
    public class IslandNPCSchedule : MonoBehaviour
    {
        [SerializeField]
        private Shopkeeper keeper;

        private void Awake()
        {
            keeper = null;
        }

        public void Update()
        {
            if (!keeper) return;

            if (Sun.sun.localTime >= 18f || Sun.sun.localTime < 7f)
            {
                if (keeper.gameObject.activeInHierarchy)
                {
                    keeper.gameObject.SetActive(false);
                }
            }
            else if (!keeper.gameObject.activeInHierarchy)
            {
                keeper.gameObject.SetActive(true);
            }
        }
    }
}
