// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "Externs.h"
#include "opensteamworks\CSteamAPIContext.h"

CSteamAPIContext* g_pApiContext = NULL;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

void Shutdown()
{
	SteamAPI_Shutdown();
}

bool Initialize()
{
	/*TSteamError error;
	SteamCallHandle_t handle;
	int value;
	
	value = SteamStartEngine(&error);
	value = SteamStartup(0xf, &error);
		
	handle = SteamLogin("rleidle", "", 1, &error);*/

	if (!SteamAPI_InitSafe())
	{
		return false;
	}

	g_pApiContext = new CSteamAPIContext();

	if (g_pApiContext == NULL)
	{
		return false;
	}

	if (!g_pApiContext->Init())
	{
		delete g_pApiContext;
		g_pApiContext = NULL;
		return false;
	}

	//CSteamID steamId = g_pApiContext->SteamUser()->GetSteamID();

	return true;
}