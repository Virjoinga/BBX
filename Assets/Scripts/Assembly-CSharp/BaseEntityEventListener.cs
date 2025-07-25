using Bolt;
using UnityEngine;

public abstract class BaseEntityEventListener<TState, TCommand1> : BaseEntityEventListener<TState> where TState : IState where TCommand1 : Command
{
	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (command is TCommand1)
		{
			ExecuteCommand(command as TCommand1, resetState);
		}
	}

	protected abstract void ExecuteCommand(TCommand1 cmd, bool resetState);
}
public abstract class BaseEntityEventListener<TState, TCommand1, TCommand2> : BaseEntityEventListener<TState, TCommand1> where TState : IState where TCommand1 : Command where TCommand2 : Command
{
	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (command is TCommand2)
		{
			ExecuteCommand(command as TCommand2, resetState);
		}
		else
		{
			base.ExecuteCommand(command, resetState);
		}
	}

	protected abstract void ExecuteCommand(TCommand2 cmd, bool resetState);
}
public abstract class BaseEntityEventListener<TState, TCommand1, TCommand2, TCommand3> : BaseEntityEventListener<TState, TCommand1, TCommand2> where TState : IState where TCommand1 : Command where TCommand2 : Command where TCommand3 : Command
{
	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (command is TCommand3)
		{
			ExecuteCommand(command as TCommand3, resetState);
		}
		else
		{
			base.ExecuteCommand(command, resetState);
		}
	}

	protected abstract void ExecuteCommand(TCommand3 cmd, bool resetState);
}
public abstract class BaseEntityEventListener<TState, TCommand1, TCommand2, TCommand3, TCommand4> : BaseEntityEventListener<TState, TCommand1, TCommand2, TCommand3> where TState : IState where TCommand1 : Command where TCommand2 : Command where TCommand3 : Command where TCommand4 : Command
{
	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (command is TCommand4)
		{
			ExecuteCommand(command as TCommand4, resetState);
		}
		else
		{
			base.ExecuteCommand(command, resetState);
		}
	}

	protected abstract void ExecuteCommand(TCommand4 cmd, bool resetState);
}
public abstract class BaseEntityEventListener<TState, TCommand1, TCommand2, TCommand3, TCommand4, TCommand5> : BaseEntityEventListener<TState, TCommand1, TCommand2, TCommand3, TCommand4> where TState : IState where TCommand1 : Command where TCommand2 : Command where TCommand3 : Command where TCommand4 : Command where TCommand5 : Command
{
	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (command is TCommand5)
		{
			ExecuteCommand(command as TCommand5, resetState);
		}
		else
		{
			base.ExecuteCommand(command, resetState);
		}
	}

	protected abstract void ExecuteCommand(TCommand5 cmd, bool resetState);
}
public abstract class BaseEntityEventListener<TState, TCommand1, TCommand2, TCommand3, TCommand4, TCommand5, TCommand6> : BaseEntityEventListener<TState, TCommand1, TCommand2, TCommand3, TCommand4, TCommand5> where TState : IState where TCommand1 : Command where TCommand2 : Command where TCommand3 : Command where TCommand4 : Command where TCommand5 : Command where TCommand6 : Command
{
	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (command is TCommand6)
		{
			ExecuteCommand(command as TCommand6, resetState);
		}
		else
		{
			base.ExecuteCommand(command, resetState);
		}
	}

	protected abstract void ExecuteCommand(TCommand6 cmd, bool resetState);
}
public abstract class BaseEntityEventListener<TState> : EntityEventListener<TState> where TState : IState
{
	[SerializeField]
	private bool _destroyIfNotAttached;

	protected virtual void Awake()
	{
		if (_destroyIfNotAttached)
		{
			Invoke("DestroyIfNotAttached", 2f);
		}
	}

	protected virtual void Start()
	{
	}

	public override void Attached()
	{
		if (_destroyIfNotAttached)
		{
			CancelInvoke("DestroyIfNotAttached");
		}
		OnAnyAttached();
		if (base.entity.isOwner)
		{
			OnOwnerOnlyAttached();
		}
		else
		{
			OnControllerOrRemoteAttached();
			if (base.entity.isControlled)
			{
				OnControllerOnlyAttached();
			}
			else
			{
				OnRemoteOnlyAttached();
			}
		}
		if (base.entity.isOwner || base.entity.isControlled)
		{
			OnOwnerOrControllerAttached();
		}
	}

	private void DestroyIfNotAttached()
	{
		Object.Destroy(base.gameObject);
	}

	protected virtual void OnAnyAttached()
	{
	}

	protected virtual void OnOwnerOnlyAttached()
	{
	}

	protected virtual void OnControllerOnlyAttached()
	{
	}

	protected virtual void OnRemoteOnlyAttached()
	{
	}

	protected virtual void OnOwnerOrControllerAttached()
	{
	}

	protected virtual void OnControllerOrRemoteAttached()
	{
	}

	public override void Detached()
	{
		OnAnyDetached();
		if (base.entity.isOwner)
		{
			OnOwnerOnlyDetached();
		}
		else
		{
			OnControllerOrRemoteDetached();
			if (base.entity.isControlled)
			{
				OnControllerOnlyDetached();
			}
			else
			{
				OnRemoteOnlyDetached();
			}
		}
		if (base.entity.isOwner || base.entity.isControlled)
		{
			OnOwnerOrControllerDetached();
		}
	}

	protected virtual void OnAnyDetached()
	{
	}

	protected virtual void OnOwnerOnlyDetached()
	{
	}

	protected virtual void OnControllerOnlyDetached()
	{
	}

	protected virtual void OnRemoteOnlyDetached()
	{
	}

	protected virtual void OnOwnerOrControllerDetached()
	{
	}

	protected virtual void OnControllerOrRemoteDetached()
	{
	}
}
