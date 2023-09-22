/******************************************
 * Class name:
 * Author:
 * Creation:
 * Last modify:
 * Version:
 * 
 * DESCRIPTION
 * 
 * 
 * *****************************************/
namespace Device.Polhemus
{
    public enum PDICONNECTIONSTATUS:int
    {
        NOT_INITIALIZED,
        SEARCHING_DEVICE,
        GET_STATION_MAP,
        READ_SINGLE_FRAME,
        SER_DATA_LIST,
        START_CONT_PNO,
        ACQUIRING,
        CLOSE_THREAD,
        STOP_ACQUISITION,
        CLEAR_PNOBUFFER,
        DISCONNECTING,
        END
    }
}
