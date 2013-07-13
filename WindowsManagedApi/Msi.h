#pragma once

#include <Windows.h>
#include <Msi.h>
#include <vcclr.h>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace WindowsManagedApi {

public value struct Msi
{
	static const int MaxFeatureLength = 38;
    static const int MaxGuidLength = 38;
    static const int MaxPathLength = 1024;

	enum class InstallState
    {
        NotUsed = -7,
        BadConfig = -6,
        Incomplete = -5,
        SourceAbsent = -4,
        MoreData = -3,
        InvalidArg = -2,
        Unknown = -1,
        Broken = 0,
        Advertised = 1,
        Removed = 1,
        Absent = 2,
        Local = 3,
        Source = 4,
        Default = 5
    };

	static bool MsiGetShortcutTarget(String^ targetFile, [Out] String^% productCode, [Out] String^% featureID, [Out] String^% componentCode)
	{
		pin_ptr<const TCHAR> utargetFile = PtrToStringChars(targetFile);
		TCHAR guidBuffer[MaxGuidLength + 1];
		TCHAR featureIdBuffer[MaxFeatureLength + 1];
		TCHAR componentCodeBuffer[MaxGuidLength + 1];
		UINT result = ::MsiGetShortcutTarget(utargetFile, guidBuffer, featureIdBuffer, componentCodeBuffer);
		productCode = gcnew String(guidBuffer);
		featureID = gcnew String(featureIdBuffer);
		componentCode = gcnew String(componentCodeBuffer);
		return result == ERROR_SUCCESS;
	}

    static InstallState MsiGetComponentPath(String^ productCode, String^ componentCode, [Out] String^% componentPath)
	{
		pin_ptr<const TCHAR> uproductCode = PtrToStringChars(productCode);
		pin_ptr<const TCHAR> ucomponentCode = PtrToStringChars(componentCode);
		TCHAR componentPathBuffer[MaxPathLength + 1];
		INSTALLSTATE result = ::MsiGetComponentPath(uproductCode, ucomponentCode, componentPathBuffer, NULL);
		componentPath = gcnew String(componentPathBuffer);
		return (InstallState)result;
	}
};

}