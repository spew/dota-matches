// steam_api_wrapper.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "steam_api_wrapper.h"
#include "opensteamworks\SteamAPI.h"


// This is an example of an exported variable
STEAM_API_WRAPPER_API int nsteam_api_wrapper=0;

// This is an example of an exported function.
STEAM_API_WRAPPER_API int fnsteam_api_wrapper(void)
{
	return 42;
}

// This is the constructor of a class that has been exported.
// see steam_api_wrapper.h for the class definition
Csteam_api_wrapper::Csteam_api_wrapper()
{
	return;
}
