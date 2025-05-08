using UnityEngine;

namespace RadFixes
{
    public class RF_NPCSchedule : MonoBehaviour
    {
        [SerializeField]
        private Shopkeeper _keeper;

        private void Awake()
        {
            _keeper = null;
        }

        public void Update()
        {
            if (!_keeper) return;

            if (Sun.sun.localTime >= 18f || Sun.sun.localTime < 7f)
            {
                if (_keeper.gameObject.activeInHierarchy)
                {
                    _keeper.gameObject.SetActive(false);
                }
            }
            else if (!_keeper.gameObject.activeInHierarchy)
            {
                _keeper.gameObject.SetActive(true);
            }
        }
    }
}
