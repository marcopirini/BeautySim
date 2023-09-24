namespace Device.Motion
{
    public class WarningMessage
    {
        public Enum_WarningMessage MessageType;
        public string message;

        public WarningMessage(Enum_WarningMessage messageType)
        {
            MessageType = messageType;
        }
    }
}
