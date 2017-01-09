local transform;
local gameObject;

LoginPanel = {};
local this = LoginPanel;

function LoginPanel.Awake( obj )
	transform = obj.transform;
	gameObject = obj;
	this.InitPanel();
end

function LoginPanel.InitPanel( )
	this.btnLogin = transform:FindChild("login").gameObject;
end

function LoginPanel.OnDestroy(  )
	logWarn("OnDestroy.........");
end