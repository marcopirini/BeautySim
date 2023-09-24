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
namespace Device.Motion
{
    public enum CONNECTIONSTATUS:int
    {
        NOT_INITIALIZED,
        SEARCHING_DEVICE,    
        INITIALIZING,   
        ACQUIRING,              
        DISCONNECTING,
        END,
        INITIALIZED,
        ERROR
    }
}
