using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ErrorCode
    {
        [XmlEnum("NoError")] NoError = 0,
        [XmlEnum("CriticalError")] CriticalError = 1,
        [XmlEnum("NonCriticalError")] NonCriticalError = 100, // 0x00000064
        [XmlEnum("Warning")] Warning = 200, // 0x000000C8
        [XmlEnum("AuthenticationJWTFailed")] AuthenticationJWTFailed = 400, // 0x00000190
        [XmlEnum("RecaptchaFailed")] RecaptchaFailed = 401, // 0x00000191
        [XmlEnum("RecaptchaFailedThreeTimes")] RecaptchaFailedThreeTimes = 402, // 0x00000192
    }
}
