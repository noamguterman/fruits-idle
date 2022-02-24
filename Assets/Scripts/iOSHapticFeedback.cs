using System;
using UnityEngine;

public class iOSHapticFeedback : MonoBehaviour
{
    private static iOSHapticFeedback _instance;

    public iOSFeedbackTypeSettings usedFeedbackTypes = new iOSFeedbackTypeSettings();

    private bool feedbackGeneratorsSetUp;

    public bool debug;

    [Serializable]
    public class iOSFeedbackTypeSettings
    {
        public bool Notifications
        {
            get
            {
                return NotificationSuccess || NotificationWarning || NotificationFailure;
            }
        }

        public bool SelectionChange = true;

        public bool ImpactLight = true;

        public bool ImpactMedium = true;

        public bool ImpactHeavy = true;

        public bool NotificationSuccess = true;

        public bool NotificationWarning = true;

        public bool NotificationFailure = true;
    }

    public enum iOSFeedbackType
    {
        SelectionChange,
        ImpactLight,
        ImpactMedium,
        ImpactHeavy,
        Success,
        Warning,
        Failure,
        None
    }
    public static iOSHapticFeedback Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = new GameObject("iOS Haptic Feedback").AddComponent<iOSHapticFeedback>();
			}
			return _instance;
		}
	}

	public static void Create()
	{
		iOSHapticFeedback instance = _instance;
	}

	protected virtual void Awake()
	{
		if (_instance)
		{
			Debug.LogWarning("There is already an instance of ");
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		_instance = this;
		for (int i = 0; i < 5; i++)
		{
			if (FeedbackIdSet(i))
			{
				InstantiateFeedbackGenerator(i);
			}
		}
		feedbackGeneratorsSetUp = true;
	}

	protected void OnDestroy()
	{
		if (!feedbackGeneratorsSetUp)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if (FeedbackIdSet(i))
			{
				ReleaseFeedbackGenerator(i);
			}
		}
	}

	protected bool FeedbackIdSet(int id)
	{
		return (id == 0 && usedFeedbackTypes.SelectionChange) || (id == 1 && usedFeedbackTypes.ImpactLight) || (id == 2 && usedFeedbackTypes.ImpactMedium) || (id == 3 && usedFeedbackTypes.ImpactHeavy) || ((id == 4 || id == 5 || id == 6) && usedFeedbackTypes.Notifications);
	}

	private void _instantiateFeedbackGenerator(int id)
	{
	}

	private void _prepareFeedbackGenerator(int id)
	{
	}

	private void _triggerFeedbackGenerator(int id, bool advanced)
	{
	}

	private void _releaseFeedbackGenerator(int id)
	{
	}

	protected void InstantiateFeedbackGenerator(int id)
	{
		if (debug)
		{
			Debug.Log("Instantiate iOS feedback generator " + (iOSFeedbackType)id);
		}
		_instantiateFeedbackGenerator(id);
	}

	protected void PrepareFeedbackGenerator(int id)
	{
		if (debug)
		{
			Debug.Log("Prepare iOS feedback generator " + (iOSFeedbackType)id);
		}
		_prepareFeedbackGenerator(id);
	}

	protected void TriggerFeedbackGenerator(int id, bool advanced)
	{
		if (debug)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Trigger iOS feedback generator ",
				(iOSFeedbackType)id,
				", advanced mode: ",
				advanced.ToString()
			}));
		}
		_triggerFeedbackGenerator(id, advanced);
	}

	protected void ReleaseFeedbackGenerator(int id)
	{
		if (debug)
		{
			Debug.Log("Release iOS feedback generator " + (iOSFeedbackType)id);
		}
		_releaseFeedbackGenerator(id);
	}

	public virtual void Trigger(iOSFeedbackType feedbackType)
	{
		if (FeedbackIdSet((int)feedbackType))
		{
			TriggerFeedbackGenerator((int)feedbackType, false);
			return;
		}
		Debug.LogError("You cannot trigger a feedback generator without instantiating it first");
	}

	public bool IsSupported()
	{
		return false;
	}

	
}
