 
 
 
 
 
 
 
 
  
 
 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 
 
namespace KP.Register.Business.Identity 
{  
    public enum Identity 
    { 
	  
			CITID ,
      
			DRILICEN ,
      
			EMBOSSID ,
      
			MID ,
      
			SHOPCARD ,
      
	}

	public enum ContactRegister
	{
	 	  
			EMAIL ,
      
			MOBILE ,
      
			WECHAT ,
      
			MOBILECHN ,
      
	}

	public enum PlatformCode
	{
		  
				POS ,
		  
				KIOSK ,
		  
				MOBILE ,
		  
				AIRPORT ,
		  
				FRMTOUR ,
		  
				FRMFIT ,
		  
				ONLINEMOB ,
		  
				ONLINE ,
		  
				ONLMOBMHN ,
		  
				ONLMOBPTY ,
		  
	}

		public enum ActionCode
	{
		  
				PRE_REGISTER_ADD ,
		  
				PRE_REGISTER_EDIT ,
		  
				REGISTER_ADD ,
		  
				REGISTER_EDIT ,
		  
	}
}