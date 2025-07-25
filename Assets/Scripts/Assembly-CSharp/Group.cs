using System.Collections.Generic;
using System.Linq;

public struct Group
{
	public static Group Empty { get; private set; } = new Group
	{
		Id = "",
		LeaderId = "",
		IAmLeader = true,
		Members = new List<GroupMember>()
	};

	public bool IsValid { get; private set; }

	public string Id { get; private set; }

	public string LeaderId { get; private set; }

	public bool IAmLeader { get; private set; }

	public List<GroupMember> Members { get; private set; }

	public Group(GroupCrumb crumb, string myId)
	{
		Id = crumb.i;
		IsValid = !string.IsNullOrEmpty(Id);
		LeaderId = crumb.l;
		IAmLeader = string.IsNullOrEmpty(LeaderId) || LeaderId == myId;
		if (crumb.m != null)
		{
			Members = crumb.m.Select((GroupMemberCrumb member) => new GroupMember(member)).ToList();
		}
		else
		{
			Members = new List<GroupMember>();
		}
	}

	public override string ToString()
	{
		List<GroupMember> source = Members ?? new List<GroupMember>();
		return "Group(Id: " + Id + ", LeaderId: " + LeaderId + ", Members: [" + string.Join(", ", source.Select((GroupMember m) => m.ToString())) + "])";
	}
}
