using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReservationScreen : MonoBehaviour
{
    [SerializeField]
    private ModalCalendar modalCalendarDialog = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowModalCalendar()
    {
        modalCalendarDialog.Show(() => {
            
        }, null);
    }
}
