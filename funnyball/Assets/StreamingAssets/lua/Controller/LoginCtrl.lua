require "Common/define"

LoginCtrl = {};
local this = LoginCtrl;

local login;
local transform;
local gameObject;

function LoginCtrl.New()
	return this;
end

function LoginCtrl.Awake()
	logWarn("LoginCtrl Awake");
	PanelManager:CreatePanel('Login', this.OnCreate);
end

function LoginCtrl.OnCreate( obj )
	gameObject = obj;
	transform = obj.transform;
	login = transform:GetComponent('LuaBehaviour'); 
	logWarn("Start lua "..gameObject.name);

	login:AddClick(LoginPanel.btnLogin, this.OnLogin);
end

function LoginCtrl.OnLogin( go )
	logWarn("in function OnLogin");
end

function LoginCtrl.InitPanel( prefab )
	-- body
end