package md55c71e2e7203b1f53ef970ac328992a03;


public class ScaleImageViewGestureDetector
	extends android.view.GestureDetector.SimpleOnGestureListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDown:(Landroid/view/MotionEvent;)Z:GetOnDown_Landroid_view_MotionEvent_Handler\n" +
			"n_onDoubleTap:(Landroid/view/MotionEvent;)Z:GetOnDoubleTap_Landroid_view_MotionEvent_Handler\n" +
			"";
		mono.android.Runtime.register ("SampleScaleImage.ScaleImageViewGestureDetector, HappinessChecker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ScaleImageViewGestureDetector.class, __md_methods);
	}


	public ScaleImageViewGestureDetector () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ScaleImageViewGestureDetector.class)
			mono.android.TypeManager.Activate ("SampleScaleImage.ScaleImageViewGestureDetector, HappinessChecker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public ScaleImageViewGestureDetector (md55c71e2e7203b1f53ef970ac328992a03.ScaleImageView p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == ScaleImageViewGestureDetector.class)
			mono.android.TypeManager.Activate ("SampleScaleImage.ScaleImageViewGestureDetector, HappinessChecker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "SampleScaleImage.ScaleImageView, HappinessChecker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public boolean onDown (android.view.MotionEvent p0)
	{
		return n_onDown (p0);
	}

	private native boolean n_onDown (android.view.MotionEvent p0);


	public boolean onDoubleTap (android.view.MotionEvent p0)
	{
		return n_onDoubleTap (p0);
	}

	private native boolean n_onDoubleTap (android.view.MotionEvent p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
