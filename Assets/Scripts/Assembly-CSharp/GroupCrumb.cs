public struct GroupCrumb
{
	public string i;

	public string l;

	public GroupMemberCrumb[] m;

	public Group ToGroup(string myId)
	{
		return new Group(this, myId);
	}
}
