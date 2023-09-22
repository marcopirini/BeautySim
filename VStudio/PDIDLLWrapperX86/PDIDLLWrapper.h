// PDIDLLWrapper.h
#include "PDI.h"

#include <iostream>
#pragma once
extern "C"{

#define PDIDLL_API __declspec(dllexport) 


	PDIDLL_API CPDIdev* Create()
	{ 
		CPDIdev* cPDIdev = new CPDIdev();

		
		return cPDIdev;
	
	}

	PDIDLL_API bool Connect(CPDIdev* instance)
	{
	
		return instance->ConnectUSB();
	}

	PDIDLL_API BOOL Disconnect(CPDIdev* instance)
	{
		return instance->Disconnect();
	}

	
	

	PDIDLL_API void Delete(CPDIdev* instance)
	{
		if (instance != NULL)
		{
			instance->Disconnect();
			delete instance;
		}
	}

	PDIDLL_API ePiCommType DiscoverCnx(CPDIdev* instance)
	{
		return instance->DiscoverCnx();
	}

	PDIDLL_API BOOL SetMetric(CPDIdev* instance, bool value)
	{
		return instance->SetMetric(value);
	}

	

	PDIDLL_API BOOL GetStationMap(CPDIdev* instance, int &dwMap)
	{		
		
		return instance->GetStationMap((DWORD&)dwMap);
	}

	PDIDLL_API BOOL ResetTracker(CPDIdev* instance)
	{
		return instance->ResetTracker();
	}


	PDIDLL_API BOOL CnxReady(CPDIdev* instance)
	{
		return instance->CnxReady();
	}

	PDIDLL_API BOOL StopContPno(CPDIdev* instance)
	{
		return instance->StopContPno();

	}

	PDIDLL_API BOOL SetDataList(CPDIdev* instance)
	{
		CPDImdat    g_pdiMDat;
		g_pdiMDat.Empty();
		g_pdiMDat.Append(PDI_MODATA_FRAMECOUNT);
		g_pdiMDat.Append(PDI_MODATA_POS);
		g_pdiMDat.Append(PDI_MODATA_QTRN);
		//g_dwFrameSize = 8 + 12 + 12 + 4;
		return instance->SetSDataList(-1, g_pdiMDat);
	}

	PDIDLL_API BOOL  GetFrameRate(CPDIdev* instance,  ePiFrameRate &FR)
	{
		
		return instance->GetFrameRate(FR);
	}
	PDIDLL_API ePiErrCode GetLastResult(CPDIdev* instance)
	{
		return (ePiErrCode)instance->GetLastResult();
	}

	PDIDLL_API BOOL ReadSinglePnoBuf(CPDIdev* instance, BYTE * buf, DWORD &dwSize)
	{		
		PBYTE pdiBuf;
		bool res = instance->ReadSinglePnoBuf(pdiBuf, dwSize);
		if (res)
			memcpy((void*)buf, (void*)pdiBuf, dwSize);
		return res;		
	}

	PDIDLL_API BOOL ReadSinglePno(CPDIdev* instance,HWND hwnd)
	{
		return instance->ReadSinglePno(hwnd);
	}


	PDIDLL_API BOOL LastHostFrameCount(CPDIdev* instance, DWORD &FC)
	{		
		return instance->LastHostFrameCount(FC);
	}

	PDIDLL_API BOOL LastPnoPtr(CPDIdev* instance, BYTE * buf, DWORD &dwSize)
	{				
		PBYTE pdiBuf;
		bool res =  instance->LastPnoPtr(pdiBuf, dwSize);
		if (res)
			memcpy((void*)buf, (void*)pdiBuf, dwSize);
		return res;
	}


	PDIDLL_API BOOL SetPnoBuffer(CPDIdev* instance, BYTE * buf, DWORD dwSize)
	{
		
		return instance->SetPnoBuffer(buf, dwSize);

	}


	PDIDLL_API BOOL ResetPnoPtr(CPDIdev* instance)
	{		
		return instance->ResetPnoPtr();
	}

	PDIDLL_API BOOL ClearPnoBuffer(CPDIdev* instance)
	{
		return instance->ClearPnoBuffer();
	}


	
	PDIDLL_API BOOL StartContPno(CPDIdev* instance, HWND handle)
	{
		return instance->StartContPno(NULL);
	}

	/*

		
		////////////////////////////////////////////////////////////////////
		int FrameSize()
		{
			return m_pMDat->FrameSize();
		}
		int NumItems()
		{
			return m_pMDat->NumItems();
		}
			
		G4ePDIMotionData ItemAt(int j)
		{
			return (G4ePDIMotionData) m_pMDat->ItemAt(j);
		}
		
		void Empty()
		{
			m_pMDat->Empty();
		}

		void Append(G4ePDIMotionData item)
		{
			m_pMDat->Append((ePDIMotionData) item);
		}
		bool SetSDataList()
		{
			return m_g4->SetSDataList(PDI_ALL_STATIONS, *(&m_pMDat));
		}
		
		bool GetActiveHubs(int % num_hubs)
		{
			
			pin_ptr<int> pnum_hubs = &num_hubs;
			int HubIDs[256];
			return m_g4->GetActiveHubs( (*pnum_hubs),HubIDs);
		}
		
	
	
	*/
	
}