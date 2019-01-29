using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum FriendType
    {
        [XmlEnum("Friend")] FriendTypeFriend,
        [XmlEnum("Ignore")] FriendTypeIgnore,
        [XmlEnum("Invited")] FriendTypeInvited,
        [XmlEnum("Approval")] FriendTypeApproval,
        [XmlEnum("Abuse")] FriendTypeAbuse,
        [XmlEnum("FacebookFriend")] FriendTypeFacebookFriend,
        [XmlEnum("Unfriended")] FriendTypeUnFriended,
    }
}