public struct GroupMember
{
	public string Id { get; private set; }

	public string TitleId { get; private set; }

	public string DisplayName { get; private set; }

	public GroupMember(GroupMemberCrumb crumb)
	{
		Id = crumb.i;
		TitleId = crumb.t;
		DisplayName = crumb.n;
	}

	public override string ToString()
	{
		return "GroupMember(Id: " + Id + ", Displayname: " + DisplayName + ")";
	}
}
