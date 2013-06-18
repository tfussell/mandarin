#region DragDropLibCore\DragDropHelper.cs

namespace DragDropLib
{
    using System.Runtime.InteropServices;

    [ComImport]
    [Guid("4657278A-411B-11d2-839A-00C04FD918D0")]
    public class DragDropHelper { }
}

#endregion // DragDropLibCore\DragDropHelper.cs

#region DragDropLibCore\IDropTargetHelper.cs

namespace DragDropLib
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    [ComVisible(true)]
    [ComImport]
    [Guid("4657278B-411B-11D2-839A-00C04FD918D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropTargetHelper
    {
        void DragEnter(
            [In] IntPtr hwndTarget,
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject,
            [In] ref Win32Point pt,
            [In] int effect);

        void DragLeave();

        void DragOver(
            [In] ref Win32Point pt,
            [In] int effect);

        void Drop(
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject,
            [In] ref Win32Point pt,
            [In] int effect);

        void Show(
            [In] bool show);
    }
}

#endregion // DragDropLibCore\IDropTargetHelper.cs

#region DragDropLibCore\NativeStructures.cs

namespace DragDropLib
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Size
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShDragImage
    {
        public Win32Size sizeDragImage;
        public Win32Point ptOffset;
        public IntPtr hbmpDragImage;
        public int crColorKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 1044)]
    public struct DropDescription
    {
        public int type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szMessage;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szInsert;
    }
}

#endregion // DragDropLibCore\NativeStructures.cs

#region DragDropLibCore\DataObjectExtensions.cs

namespace System.Runtime.InteropServices.ComTypes
{
    using System;
    using InteropServices;
    using DragDropLib;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Provides extended functionality for the COM IDataObject interface.
    /// </summary>
    public static class ComDataObjectExtensions
    {
        #region DLL imports

        [DllImport("user32.dll")]
        private static extern uint RegisterClipboardFormat(string lpszFormatName);

        [DllImport("ole32.dll")]
        private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

        [DllImport("ole32.dll")]
        private static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out IStream ppstm);

        #endregion // DLL imports

        #region Native constants

        // CFSTR_DROPDESCRIPTION
        private const string DropDescriptionFormat = "DropDescription";

        #endregion // Native constants

        /// <summary>
        /// Sets the drop description for the drag image manager.
        /// </summary>
        /// <param name="dataObject">The DataObject to set.</param>
        /// <param name="dropDescription">The drop description.</param>
        public static void SetDropDescription(this IDataObject dataObject, DropDescription dropDescription)
        {
            ComTypes.FORMATETC formatETC;
            FillFormatETC(DropDescriptionFormat, TYMED.TYMED_HGLOBAL, out formatETC);

            // We need to set the drop description as an HGLOBAL.
            // Allocate space ...
            IntPtr pDD = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DropDescription)));
            try
            {
                // ... and marshal the data
                Marshal.StructureToPtr(dropDescription, pDD, false);

                // The medium wraps the HGLOBAL
                System.Runtime.InteropServices.ComTypes.STGMEDIUM medium;
                medium.pUnkForRelease = null;
                medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
                medium.unionmember = pDD;

                // Set the data
                var dataObjectCOM = (ComTypes.IDataObject)dataObject;
                dataObjectCOM.SetData(ref formatETC, ref medium, true);
            }
            catch
            {
                // If we failed, we need to free the HGLOBAL memory
                Marshal.FreeHGlobal(pDD);
                throw;
            }
        }

        /// <summary>
        /// Gets the DropDescription format data.
        /// </summary>
        /// <param name="dataObject">The DataObject.</param>
        /// <returns>The DropDescription, if set.</returns>
        public static object GetDropDescription(this IDataObject dataObject)
        {
            ComTypes.FORMATETC formatETC;
            FillFormatETC(DropDescriptionFormat, TYMED.TYMED_HGLOBAL, out formatETC);

            if (0 == dataObject.QueryGetData(ref formatETC))
            {
                ComTypes.STGMEDIUM medium;
                dataObject.GetData(ref formatETC, out medium);
                try
                {
                    return (DropDescription)Marshal.PtrToStructure(medium.unionmember, typeof(DropDescription));
                }
                finally
                {
                    ReleaseStgMedium(ref medium);
                }
            }

            return null;
        }

        // Combination of all non-null TYMEDs
        private const TYMED TYMED_ANY =
            TYMED.TYMED_ENHMF
            | TYMED.TYMED_FILE
            | TYMED.TYMED_GDI
            | TYMED.TYMED_HGLOBAL
            | TYMED.TYMED_ISTORAGE
            | TYMED.TYMED_ISTREAM
            | TYMED.TYMED_MFPICT;

        /// <summary>
        /// Sets up an advisory connection to the data object.
        /// </summary>
        /// <param name="dataObject">The data object on which to set the advisory connection.</param>
        /// <param name="sink">The advisory sink.</param>
        /// <param name="format">The format on which to callback on.</param>
        /// <param name="advf">Advisory flags. Can be 0.</param>
        /// <returns>The ID of the newly created advisory connection.</returns>
        public static int Advise(this IDataObject dataObject, IAdviseSink sink, string format, ADVF advf)
        {
            // Internally, we'll listen for any TYMED
            FORMATETC formatETC;
            FillFormatETC(format, TYMED_ANY, out formatETC);

            int connection;
            int hr = dataObject.DAdvise(ref formatETC, advf, sink, out connection);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
            return connection;
        }

        /// <summary>
        /// Fills a FORMATETC structure.
        /// </summary>
        /// <param name="format">The format name.</param>
        /// <param name="tymed">The accepted TYMED.</param>
        /// <param name="formatETC">The structure to fill.</param>
        private static void FillFormatETC(string format, TYMED tymed, out FORMATETC formatETC)
        {
            formatETC.cfFormat = (short)RegisterClipboardFormat(format);
            formatETC.dwAspect = DVASPECT.DVASPECT_CONTENT;
            formatETC.lindex = -1;
            formatETC.ptd = IntPtr.Zero;
            formatETC.tymed = tymed;
        }

        // Identifies data that we need to do custom marshaling on
        private static readonly Guid ManagedDataStamp = new Guid("D98D9FD6-FA46-4716-A769-F3451DFBE4B4");

        /// <summary>
        /// Sets managed data to a clipboard DataObject.
        /// </summary>
        /// <param name="dataObject">The DataObject to set the data on.</param>
        /// <param name="format">The clipboard format.</param>
        /// <param name="data">The data object.</param>
        /// <remarks>
        /// Because the underlying data store is not storing managed objects, but
        /// unmanaged ones, this function provides intelligent conversion, allowing
        /// you to set unmanaged data into the COM implemented IDataObject.</remarks>
        public static void SetManagedData(this IDataObject dataObject, string format, object data)
        {
            // Initialize the format structure
            ComTypes.FORMATETC formatETC;
            FillFormatETC(format, TYMED.TYMED_HGLOBAL, out formatETC);

            // Serialize/marshal our data into an unmanaged medium
            ComTypes.STGMEDIUM medium;
            GetMediumFromObject(data, out medium);
            try
            {
                // Set the data on our data object
                dataObject.SetData(ref formatETC, ref medium, true);
            }
            catch
            {
                // On exceptions, release the medium
                ReleaseStgMedium(ref medium);
                throw;
            }
        }

        /// <summary>
        /// Gets managed data from a clipboard DataObject.
        /// </summary>
        /// <param name="dataObject">The DataObject to obtain the data from.</param>
        /// <param name="format">The format for which to get the data in.</param>
        /// <returns>The data object instance.</returns>
        public static object GetManagedData(this IDataObject dataObject, string format)
        {
            FORMATETC formatETC;
            FillFormatETC(format, TYMED.TYMED_HGLOBAL, out formatETC);

            // Get the data as a stream
            STGMEDIUM medium;
            dataObject.GetData(ref formatETC, out medium);

            IStream nativeStream;
            try
            {
                int hr = CreateStreamOnHGlobal(medium.unionmember, true, out nativeStream);
                if (hr != 0)
                {
                    return null;
                }
            }
            finally
            {
                ReleaseStgMedium(ref medium);
            }


            // Convert the native stream to a managed stream            
            STATSTG statstg;
            nativeStream.Stat(out statstg, 0);
            if (statstg.cbSize > int.MaxValue)
                throw new NotSupportedException();
            var buf = new byte[statstg.cbSize];
            nativeStream.Read(buf, (int)statstg.cbSize, IntPtr.Zero);
            MemoryStream dataStream = new MemoryStream(buf);

            // Check for our stamp
            int sizeOfGuid = Marshal.SizeOf(typeof(Guid));
            byte[] guidBytes = new byte[sizeOfGuid];
            if (dataStream.Length >= sizeOfGuid)
            {
                if (sizeOfGuid == dataStream.Read(guidBytes, 0, sizeOfGuid))
                {
                    Guid guid = new Guid(guidBytes);
                    if (ManagedDataStamp.Equals(guid))
                    {
                        // Stamp matched, so deserialize
                        BinaryFormatter formatter = new BinaryFormatter();
                        Type dataType = (Type)formatter.Deserialize(dataStream);
                        object data2 = formatter.Deserialize(dataStream);
                        if (data2.GetType() == dataType)
                            return data2;
                        else if (data2 is string)
                            return ConvertDataFromString((string)data2, dataType);
                        else
                            return null;
                    }
                }
            }

            // Stamp didn't match... attempt to reset the seek pointer
            if (dataStream.CanSeek)
                dataStream.Position = 0;
            return null;
        }

        #region Helper methods

        /// <summary>
        /// Serializes managed data to an HGLOBAL.
        /// </summary>
        /// <param name="data">The managed data object.</param>
        /// <returns>An STGMEDIUM pointing to the allocated HGLOBAL.</returns>
        private static void GetMediumFromObject(object data, out STGMEDIUM medium)
        {
            // We'll serialize to a managed stream temporarily
            MemoryStream stream = new MemoryStream();

            // Write an indentifying stamp, so we can recognize this as custom
            // marshaled data.
            stream.Write(ManagedDataStamp.ToByteArray(), 0, Marshal.SizeOf(typeof(Guid)));

            // Now serialize the data. Note, if the data is not directly serializable,
            // we'll try type conversion. Also, we serialize the type. That way,
            // during deserialization, we know which type to convert back to, if
            // appropriate.
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data.GetType());
            formatter.Serialize(stream, GetAsSerializable(data));

            // Now copy to an HGLOBAL
            byte[] bytes = stream.GetBuffer();
            IntPtr p = Marshal.AllocHGlobal(bytes.Length);
            try
            {
                Marshal.Copy(bytes, 0, p, bytes.Length);
            }
            catch
            {
                // Make sure to free the memory on exceptions
                Marshal.FreeHGlobal(p);
                throw;
            }

            // Now allocate an STGMEDIUM to wrap the HGLOBAL
            medium.unionmember = p;
            medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
            medium.pUnkForRelease = null;
        }

        /// <summary>
        /// Gets a serializable object representing the data.
        /// </summary>
        /// <param name="obj">The data.</param>
        /// <returns>If the data is serializable, then it is returned. Otherwise,
        /// type conversion is attempted. If successful, a string value will be
        /// returned.</returns>
        private static object GetAsSerializable(object obj)
        {
            // If the data is directly serializable, run with it
            if (obj.GetType().IsSerializable)
                return obj;

            // Attempt type conversion to a string, but only if we know it can be converted back
            TypeConverter conv = GetTypeConverterForType(obj.GetType());
            if (conv != null && conv.CanConvertTo(typeof(string)) && conv.CanConvertFrom(typeof(string)))
                return conv.ConvertToInvariantString(obj);

            throw new NotSupportedException("Cannot serialize the object");
        }

        /// <summary>
        /// Converts data from a string to the specified format.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <param name="dataType">The target data type.</param>
        /// <returns>Returns the converted data instance.</returns>
        private static object ConvertDataFromString(string data, Type dataType)
        {
            TypeConverter conv = GetTypeConverterForType(dataType);
            if (conv != null && conv.CanConvertFrom(typeof(string)))
                return conv.ConvertFromInvariantString(data);

            throw new NotSupportedException("Cannot convert data");
        }

        /// <summary>
        /// Gets a TypeConverter instance for the specified type.
        /// </summary>
        /// <param name="dataType">The type.</param>
        /// <returns>An instance of a TypeConverter for the type, if one exists.</returns>
        private static TypeConverter GetTypeConverterForType(Type dataType)
        {
            TypeConverterAttribute[] typeConverterAttrs = (TypeConverterAttribute[])dataType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            if (typeConverterAttrs.Length > 0)
            {
                Type convType = Type.GetType(typeConverterAttrs[0].ConverterTypeName);
                return (TypeConverter)Activator.CreateInstance(convType);
            }

            return null;
        }

        #endregion // Helper methods
    }
}

#endregion // DragDropLibCore\DataObjectExtensions.cs

#region DragDropLibCore\DataObject.cs

namespace DragDropLib
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    /// <summary>
    /// Implements the COM version of IDataObject including SetData.
    /// </summary>
    /// <remarks>
    /// <para>Use this object when using shell (or other unmanged) features
    /// that utilize the clipboard and/or drag and drop.</para>
    /// <para>The System.Windows.DataObject (.NET 3.0) and
    /// System.Windows.Forms.DataObject do not support SetData from their COM
    /// IDataObject interface implementation.</para>
    /// <para>To use this object with .NET drag and drop, create an instance
    /// of System.Windows.DataObject (.NET 3.0) or System.Window.Forms.DataObject
    /// passing an instance of DataObject as the only constructor parameter. For
    /// example:</para>
    /// <code>
    /// System.Windows.DataObject data = new System.Windows.DataObject(new DragDropLib.DataObject());
    /// </code>
    /// </remarks>
    [ComVisible(true)]
    public class DataObject : IDataObject, IDisposable
    {
        #region Unmanaged functions

        // These are helper functions for managing STGMEDIUM structures

        [DllImport("urlmon.dll")]
        private static extern int CopyStgMedium(ref STGMEDIUM pcstgmedSrc, ref STGMEDIUM pstgmedDest);
        [DllImport("ole32.dll")]
        private static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);

        #endregion // Unmanaged functions

        // Our internal storage is a simple list
        private readonly IList<KeyValuePair<FORMATETC, STGMEDIUM>> storage;

        // Keeps a progressive unique connection id
        private int nextConnectionId = 1;

        // List of advisory connections
        private readonly IDictionary<int, AdviseEntry> connections;

        // Represents an advisory connection entry.
        private class AdviseEntry
        {
            public readonly FORMATETC format;
            public readonly ADVF advf;
            public readonly IAdviseSink sink;

            public AdviseEntry(ref FORMATETC format, ADVF advf, IAdviseSink sink)
            {
                this.format = format;
                this.advf = advf;
                this.sink = sink;
            }
        }

        /// <summary>
        /// Creates an empty instance of DataObject.
        /// </summary>
        public DataObject()
        {
            storage = new List<KeyValuePair<FORMATETC, STGMEDIUM>>();
            connections = new Dictionary<int, AdviseEntry>();
        }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        ~DataObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Clears the internal storage array.
        /// </summary>
        /// <remarks>
        /// ClearStorage is called by the IDisposable.Dispose method implementation
        /// to make sure all unmanaged references are released properly.
        /// </remarks>
        private void ClearStorage()
        {
            foreach (KeyValuePair<FORMATETC, STGMEDIUM> pair in storage)
            {
                STGMEDIUM medium = pair.Value;
                ReleaseStgMedium(ref medium);
            }
            storage.Clear();
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        /// <param name="disposing">Indicates if the call was made by a managed caller, or the garbage collector.
        /// True indicates that someone called the Dispose method directly. False indicates that the garbage collector
        /// is finalizing the release of the object instance.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // No managed objects to release
                GC.SuppressFinalize(this);
            }

            // Always release unmanaged objects
            ClearStorage();
        }

        #region COM IDataObject Members

        #region COM constants

        private const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);

        private const int DV_E_FORMATETC = unchecked((int)0x80040064);
        private const int DV_E_TYMED = unchecked((int)0x80040069);
        private const int DV_E_CLIPFORMAT = unchecked((int)0x8004006A);
        private const int DV_E_DVASPECT = unchecked((int)0x8004006B);

        #endregion // COM constants

        #region Unsupported functions

        public int EnumDAdvise(out IEnumSTATDATA enumAdvise)
        {
            throw Marshal.GetExceptionForHR(OLE_E_ADVISENOTSUPPORTED);
        }

        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            formatOut = formatIn;
            return DV_E_FORMATETC;
        }

        #endregion // Unsupported functions

        /// <summary>
        /// Adds an advisory connection for the specified format.
        /// </summary>
        /// <param name="pFormatetc">The format for which this sink is called for changes.</param>
        /// <param name="advf">Advisory flags to specify callback behavior.</param>
        /// <param name="adviseSink">The IAdviseSink to call for this connection.</param>
        /// <param name="connection">Returns the new connection's ID.</param>
        /// <returns>An HRESULT.</returns>
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
        {
            // Check that the specified advisory flags are supported.
            const ADVF ADVF_ALLOWED = ADVF.ADVF_NODATA | ADVF.ADVF_ONLYONCE | ADVF.ADVF_PRIMEFIRST;
            if ((int)((advf | ADVF_ALLOWED) ^ ADVF_ALLOWED) != 0)
            {
                connection = 0;
                return OLE_E_ADVISENOTSUPPORTED;
            }

            // Create and insert an entry for the connection list
            AdviseEntry entry = new AdviseEntry(ref pFormatetc, advf, adviseSink);
            connections.Add(nextConnectionId, entry);
            connection = nextConnectionId;
            nextConnectionId++;

            // If the ADVF_PRIMEFIRST flag is specified and the data exists,
            // raise the DataChanged event now.
            if ((advf & ADVF.ADVF_PRIMEFIRST) == ADVF.ADVF_PRIMEFIRST)
            {
                KeyValuePair<FORMATETC, STGMEDIUM> dataEntry;
                if (GetDataEntry(ref pFormatetc, out dataEntry))
                    RaiseDataChanged(connection, ref dataEntry);
            }

            // S_OK
            return 0;
        }

        /// <summary>
        /// Removes an advisory connection.
        /// </summary>
        /// <param name="connection">The connection id to remove.</param>
        public void DUnadvise(int connection)
        {
            connections.Remove(connection);
        }

        /// <summary>
        /// Gets an enumerator for the formats contained in this DataObject.
        /// </summary>
        /// <param name="direction">The direction of the data.</param>
        /// <returns>An instance of the IEnumFORMATETC interface.</returns>
        public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
        {
            // We only support GET
            if (DATADIR.DATADIR_GET == direction)
                return new EnumFORMATETC(storage);

            throw new NotImplementedException("OLE_S_USEREG");
        }

        /// <summary>
        /// Gets the specified data.
        /// </summary>
        /// <param name="format">The requested data format.</param>
        /// <param name="medium">When the function returns, contains the requested data.</param>
        public void GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            medium = new STGMEDIUM();
            GetDataHere(ref format, ref medium);
        }

        /// <summary>
        /// Gets the specified data.
        /// </summary>
        /// <param name="format">The requested data format.</param>
        /// <param name="medium">When the function returns, contains the requested data.</param>
        /// <remarks>Differs from GetData only in that the STGMEDIUM storage is
        /// allocated and owned by the caller.</remarks>
        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            // Locate the data
            KeyValuePair<FORMATETC, STGMEDIUM> dataEntry;
            if (GetDataEntry(ref format, out dataEntry))
            {
                STGMEDIUM source = dataEntry.Value;
                medium = CopyMedium(ref source);
                return;
            }

            // Didn't find it
            throw Marshal.GetExceptionForHR(DV_E_FORMATETC);
        }

        /// <summary>
        /// Determines if data of the requested format is present.
        /// </summary>
        /// <param name="format">The request data format.</param>
        /// <returns>Returns the status of the request. If the data is present, S_OK is returned.
        /// If the data is not present, an error code with the best guess as to the reason is returned.</returns>
        public int QueryGetData(ref FORMATETC format)
        {
            // We only support CONTENT aspect
            if ((DVASPECT.DVASPECT_CONTENT & format.dwAspect) == 0)
                return DV_E_DVASPECT;

            int ret = DV_E_TYMED;

            // Try to locate the data
            // TODO: The ret, if not S_OK, is only relevant to the last item
            foreach (KeyValuePair<FORMATETC, STGMEDIUM> pair in storage)
            {
                if ((pair.Key.tymed & format.tymed) > 0)
                {
                    if (pair.Key.cfFormat == format.cfFormat)
                    {
                        // Found it, return S_OK;
                        return 0;
                    }
                    else
                    {
                        // Found the medium type, but wrong format
                        ret = DV_E_CLIPFORMAT;
                    }
                }
                else
                {
                    // Mismatch on medium type
                    ret = DV_E_TYMED;
                }
            }

            return ret;
        }

        /// <summary>
        /// Sets data in the specified format into storage.
        /// </summary>
        /// <param name="formatIn">The format of the data.</param>
        /// <param name="medium">The data.</param>
        /// <param name="release">If true, ownership of the medium's memory will be transferred
        /// to this object. If false, a copy of the medium will be created and maintained, and
        /// the caller is responsible for the memory of the medium it provided.</param>
        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            // If the format exists in our storage, remove it prior to resetting it
            foreach (KeyValuePair<FORMATETC, STGMEDIUM> pair in storage)
            {
                if ((pair.Key.tymed & formatIn.tymed) > 0
                    && pair.Key.dwAspect == formatIn.dwAspect
                    && pair.Key.cfFormat == formatIn.cfFormat)
                {
                    STGMEDIUM releaseMedium = pair.Value;
                    ReleaseStgMedium(ref releaseMedium);
                    storage.Remove(pair);
                    break;
                }
            }

            // If release is true, we'll take ownership of the medium.
            // If not, we'll make a copy of it.
            STGMEDIUM sm = medium;
            if (!release)
                sm = CopyMedium(ref medium);

            // Add it to the internal storage
            KeyValuePair<FORMATETC, STGMEDIUM> addPair = new KeyValuePair<FORMATETC, STGMEDIUM>(formatIn, sm);
            storage.Add(addPair);

            RaiseDataChanged(ref addPair);
        }

        /// <summary>
        /// Creates a copy of the STGMEDIUM structure.
        /// </summary>
        /// <param name="medium">The data to copy.</param>
        /// <returns>The copied data.</returns>
        private STGMEDIUM CopyMedium(ref STGMEDIUM medium)
        {
            STGMEDIUM sm = new STGMEDIUM();
            int hr = CopyStgMedium(ref medium, ref sm);
            if (hr != 0)
                throw Marshal.GetExceptionForHR(hr);

            return sm;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Gets a data entry by the specified format.
        /// </summary>
        /// <param name="pFormatetc">The format to locate the data entry for.</param>
        /// <param name="dataEntry">The located data entry.</param>
        /// <returns>True if the data entry was found, otherwise False.</returns>
        private bool GetDataEntry(ref FORMATETC pFormatetc, out KeyValuePair<FORMATETC, STGMEDIUM> dataEntry)
        {
            foreach (KeyValuePair<FORMATETC, STGMEDIUM> entry in storage)
            {
                FORMATETC format = entry.Key;
                if (IsFormatCompatible(ref pFormatetc, ref format))
                {
                    dataEntry = entry;
                    return true;
                }
            }

            // Not found... default allocate the out param
            dataEntry = default(KeyValuePair<FORMATETC, STGMEDIUM>);
            return false;
        }

        /// <summary>
        /// Raises the DataChanged event for the specified connection.
        /// </summary>
        /// <param name="connection">The connection id.</param>
        /// <param name="dataEntry">The data entry for which to raise the event.</param>
        private void RaiseDataChanged(int connection, ref KeyValuePair<FORMATETC, STGMEDIUM> dataEntry)
        {
            AdviseEntry adviseEntry = connections[connection];
            FORMATETC format = dataEntry.Key;
            STGMEDIUM medium;
            if ((adviseEntry.advf & ADVF.ADVF_NODATA) != ADVF.ADVF_NODATA)
                medium = dataEntry.Value;
            else
                medium = default(STGMEDIUM);

            adviseEntry.sink.OnDataChange(ref format, ref medium);

            if ((adviseEntry.advf & ADVF.ADVF_ONLYONCE) == ADVF.ADVF_ONLYONCE)
                connections.Remove(connection);
        }

        /// <summary>
        /// Raises the DataChanged event for any advisory connections that
        /// are listening for it.
        /// </summary>
        /// <param name="dataEntry">The relevant data entry.</param>
        private void RaiseDataChanged(ref KeyValuePair<FORMATETC, STGMEDIUM> dataEntry)
        {
            foreach (KeyValuePair<int, AdviseEntry> connection in connections)
            {
                if (IsFormatCompatible(connection.Value.format, dataEntry.Key))
                    RaiseDataChanged(connection.Key, ref dataEntry);
            }
        }

        /// <summary>
        /// Determines if the formats are compatible.
        /// </summary>
        /// <param name="format1">A FORMATETC.</param>
        /// <param name="format2">A FORMATETC.</param>
        /// <returns>True if the formats are compatible, otherwise False.</returns>
        /// <remarks>Compatible formats have the same DVASPECT, same format ID, and share
        /// at least one TYMED.</remarks>
        private bool IsFormatCompatible(FORMATETC format1, FORMATETC format2)
        {
            return IsFormatCompatible(ref format1, ref format2);
        }

        /// <summary>
        /// Determines if the formats are compatible.
        /// </summary>
        /// <param name="format1">A FORMATETC.</param>
        /// <param name="format2">A FORMATETC.</param>
        /// <returns>True if the formats are compatible, otherwise False.</returns>
        /// <remarks>Compatible formats have the same DVASPECT, same format ID, and share
        /// at least one TYMED.</remarks>
        private bool IsFormatCompatible(ref FORMATETC format1, ref FORMATETC format2)
        {
            return ((format1.tymed & format2.tymed) > 0
                    && format1.dwAspect == format2.dwAspect
                    && format1.cfFormat == format2.cfFormat);
        }

        #endregion // Helper methods

        #region EnumFORMATETC class

        /// <summary>
        /// Helps enumerate the formats available in our DataObject class.
        /// </summary>
        [ComVisible(true)]
        private class EnumFORMATETC : IEnumFORMATETC
        {
            // Keep an array of the formats for enumeration
            private readonly FORMATETC[] formats;
            // The index of the next item
            private int currentIndex = 0;

            /// <summary>
            /// Creates an instance from a list of key value pairs.
            /// </summary>
            /// <param name="storage">List of FORMATETC/STGMEDIUM key value pairs</param>
            internal EnumFORMATETC(IList<KeyValuePair<FORMATETC, STGMEDIUM>> storage)
            {
                // Get the formats from the list
                formats = new FORMATETC[storage.Count];
                for (int i = 0; i < formats.Length; i++)
                    formats[i] = storage[i].Key;
            }

            /// <summary>
            /// Creates an instance from an array of FORMATETC's.
            /// </summary>
            /// <param name="formats">Array of formats to enumerate.</param>
            private EnumFORMATETC(FORMATETC[] formats)
            {
                // Get the formats as a copy of the array
                this.formats = new FORMATETC[formats.Length];
                formats.CopyTo(this.formats, 0);
            }

            #region IEnumFORMATETC Members

            /// <summary>
            /// Creates a clone of this enumerator.
            /// </summary>
            /// <param name="newEnum">When this function returns, contains a new instance of IEnumFORMATETC.</param>
            public void Clone(out IEnumFORMATETC newEnum)
            {
                EnumFORMATETC ret = new EnumFORMATETC(formats);
                ret.currentIndex = currentIndex;
                newEnum = ret;
            }

            /// <summary>
            /// Retrieves the next elements from the enumeration.
            /// </summary>
            /// <param name="celt">The number of elements to retrieve.</param>
            /// <param name="rgelt">An array to receive the formats requested.</param>
            /// <param name="pceltFetched">An array to receive the number of element fetched.</param>
            /// <returns>If the fetched number of formats is the same as the requested number, S_OK is returned.
            /// There are several reasons S_FALSE may be returned: (1) The requested number of elements is less than
            /// or equal to zero. (2) The rgelt parameter equals null. (3) There are no more elements to enumerate.
            /// (4) The requested number of elements is greater than one and pceltFetched equals null or does not
            /// have at least one element in it. (5) The number of fetched elements is less than the number of
            /// requested elements.</returns>
            public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
            {
                // Start with zero fetched, in case we return early
                if (pceltFetched != null && pceltFetched.Length > 0)
                    pceltFetched[0] = 0;

                // This will count down as we fetch elements
                int cReturn = celt;

                // Short circuit if they didn't request any elements, or didn't
                // provide room in the return array, or there are not more elements
                // to enumerate.
                if (celt <= 0 || rgelt == null || currentIndex >= formats.Length)
                    return 1; // S_FALSE

                // If the number of requested elements is not one, then we must
                // be able to tell the caller how many elements were fetched.
                if ((pceltFetched == null || pceltFetched.Length < 1) && celt != 1)
                    return 1; // S_FALSE

                // If the number of elements in the return array is too small, we
                // throw. This is not a likely scenario, hence the exception.
                if (rgelt.Length < celt)
                    throw new ArgumentException("The number of elements in the return array is less than the number of elements requested");

                // Fetch the elements.
                for (int i = 0; currentIndex < formats.Length && cReturn > 0; i++, cReturn--, currentIndex++)
                    rgelt[i] = formats[currentIndex];

                // Return the number of elements fetched
                if (pceltFetched != null && pceltFetched.Length > 0)
                    pceltFetched[0] = celt - cReturn;

                // cReturn has the number of elements requested but not fetched.
                // It will be greater than zero, if multiple elements were requested
                // but we hit the end of the enumeration.
                return (cReturn == 0) ? 0 : 1; // S_OK : S_FALSE
            }

            /// <summary>
            /// Resets the state of enumeration.
            /// </summary>
            /// <returns>S_OK</returns>
            public int Reset()
            {
                currentIndex = 0;
                return 0; // S_OK
            }

            /// <summary>
            /// Skips the number of elements requested.
            /// </summary>
            /// <param name="celt">The number of elements to skip.</param>
            /// <returns>If there are not enough remaining elements to skip, returns S_FALSE. Otherwise, S_OK is returned.</returns>
            public int Skip(int celt)
            {
                if (currentIndex + celt > formats.Length)
                    return 1; // S_FALSE

                currentIndex += celt;
                return 0; // S_OK
            }

            #endregion
        }

        #endregion // EnumFORMATETC class
    }
}

#endregion // DragDropLibCore\DataObject.cs

#region SwfDragDropLib\SwfDataObjectExtensions.cs

namespace System.Windows.Forms
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using DragDropLib;
    using ComTypes = System.Runtime.InteropServices.ComTypes;

    public enum DropImageType
    {
        Invalid = -1,
        None = 0,
        Copy = (int)DragDropEffects.Copy,
        Move = (int)DragDropEffects.Move,
        Link = (int)DragDropEffects.Link,
        Label = 6,
        Warning = 7
    }

    /// <summary>
    /// Provides extended functionality to the System.Windows.Forms.IDataObject interface.
    /// </summary>
    public static class SwfDataObjectExtensions
    {
        #region DLL imports

        [DllImport("gdiplus.dll")]
        private static extern bool DeleteObject(IntPtr hgdi);

        [DllImport("ole32.dll")]
        private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

        #endregion // DLL imports

        /// <summary>
        /// Sets the drop description for the drag image manager.
        /// </summary>
        /// <param name="dataObject">The DataObject to set.</param>
        /// <param name="type">The type of the drop image.</param>
        /// <param name="format">The format string for the description.</param>
        /// <param name="insert">The parameter for the drop description.</param>
        /// <remarks>
        /// When setting the drop description, the text can be set in two part,
        /// which will be rendered slightly differently to distinguish the description
        /// from the subject. For example, the format can be set as "Move to %1" and
        /// the insert as "Temp". When rendered, the "%1" in format will be replaced
        /// with "Temp", but "Temp" will be rendered slightly different from "Move to ".
        /// </remarks>
        public static void SetDropDescription(this IDataObject dataObject, DropImageType type, string format, string insert)
        {
            if (format != null && format.Length > 259)
                throw new ArgumentException("Format string exceeds the maximum allowed length of 259.", "format");
            if (insert != null && insert.Length > 259)
                throw new ArgumentException("Insert string exceeds the maximum allowed length of 259.", "insert");

            // Fill the structure
            DropDescription dd;
            dd.type = (int)type;
            dd.szMessage = format;
            dd.szInsert = insert;

            ComTypes.ComDataObjectExtensions.SetDropDescription((ComTypes.IDataObject)dataObject, dd);
        }

        /// <summary>
        /// Sets managed data to a clipboard DataObject.
        /// </summary>
        /// <param name="dataObject">The DataObject to set the data on.</param>
        /// <param name="format">The clipboard format.</param>
        /// <param name="data">The data object.</param>
        /// <remarks>
        /// Because the underlying data store is not storing managed objects, but
        /// unmanaged ones, this function provides intelligent conversion, allowing
        /// you to set unmanaged data into the COM implemented IDataObject.</remarks>
        public static void SetDataEx(this IDataObject dataObject, string format, object data)
        {
            DataFormats.Format dataFormat = DataFormats.GetFormat(format);

            // Initialize the format structure
            ComTypes.FORMATETC formatETC = new ComTypes.FORMATETC();
            formatETC.cfFormat = (short)dataFormat.Id;
            formatETC.dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT;
            formatETC.lindex = -1;
            formatETC.ptd = IntPtr.Zero;

            // Try to discover the TYMED from the format and data
            ComTypes.TYMED tymed = GetCompatibleTymed(format, data);
            // If a TYMED was found, we can use the system DataObject
            // to convert our value for us.
            if (tymed != ComTypes.TYMED.TYMED_NULL)
            {
                formatETC.tymed = tymed;

                // Set data on an empty DataObject instance
                DataObject conv = new DataObject();
                conv.SetData(format, true, data);

                // Now retrieve the data, using the COM interface.
                // This will perform a managed to unmanaged conversion for us.
                ComTypes.STGMEDIUM medium;
                ((ComTypes.IDataObject)conv).GetData(ref formatETC, out medium);
                try
                {
                    // Now set the data on our data object
                    ((ComTypes.IDataObject)dataObject).SetData(ref formatETC, ref medium, true);
                }
                catch
                {
                    // On exceptions, release the medium
                    ReleaseStgMedium(ref medium);
                    throw;
                }
            }
            else
            {
                // Since we couldn't determine a TYMED, this data
                // is likely custom managed data, and won't be used
                // by unmanaged code, so we'll use our custom marshaling
                // implemented by our COM IDataObject extensions.

                ComTypes.ComDataObjectExtensions.SetManagedData((ComTypes.IDataObject)dataObject, format, data);
            }
        }

        /// <summary>
        /// Gets a system compatible TYMED for the given format.
        /// </summary>
        /// <param name="format">The data format.</param>
        /// <param name="data">The data.</param>
        /// <returns>A TYMED value, indicating a system compatible TYMED that can
        /// be used for data marshaling.</returns>
        private static ComTypes.TYMED GetCompatibleTymed(string format, object data)
        {
            if (IsFormatEqual(format, DataFormats.Bitmap) && data is System.Drawing.Bitmap)
                return ComTypes.TYMED.TYMED_GDI;
            if (IsFormatEqual(format, DataFormats.EnhancedMetafile))
                return ComTypes.TYMED.TYMED_ENHMF;
            if (data is Stream
                || IsFormatEqual(format, DataFormats.Html)
                || IsFormatEqual(format, DataFormats.Text) || IsFormatEqual(format, DataFormats.Rtf)
                || IsFormatEqual(format, DataFormats.OemText)
                || IsFormatEqual(format, DataFormats.UnicodeText) || IsFormatEqual(format, "ApplicationTrust")
                || IsFormatEqual(format, DataFormats.FileDrop)
                || IsFormatEqual(format, "FileName")
                || IsFormatEqual(format, "FileNameW"))
                return ComTypes.TYMED.TYMED_HGLOBAL;
            if (IsFormatEqual(format, DataFormats.Dib) && data is System.Drawing.Image)
                return System.Runtime.InteropServices.ComTypes.TYMED.TYMED_NULL;
            if (IsFormatEqual(format, typeof(System.Drawing.Bitmap).FullName))
                return ComTypes.TYMED.TYMED_HGLOBAL;
            if (IsFormatEqual(format, DataFormats.EnhancedMetafile) || data is System.Drawing.Imaging.Metafile)
                return System.Runtime.InteropServices.ComTypes.TYMED.TYMED_NULL;
            if (IsFormatEqual(format, DataFormats.Serializable) || (data is System.Runtime.Serialization.ISerializable)
                || ((data != null) && data.GetType().IsSerializable))
                return ComTypes.TYMED.TYMED_HGLOBAL;

            return ComTypes.TYMED.TYMED_NULL;
        }

        /// <summary>
        /// Compares the equality of two clipboard formats.
        /// </summary>
        /// <param name="formatA">First format.</param>
        /// <param name="formatB">Second format.</param>
        /// <returns>True if the formats are equal. False otherwise.</returns>
        private static bool IsFormatEqual(string formatA, string formatB)
        {
            return string.CompareOrdinal(formatA, formatB) == 0;
        }

        /// <summary>
        /// Gets managed data from a clipboard DataObject.
        /// </summary>
        /// <param name="dataObject">The DataObject to obtain the data from.</param>
        /// <param name="format">The format for which to get the data in.</param>
        /// <returns>The data object instance.</returns>
        public static object GetDataEx(this IDataObject dataObject, string format)
        {
            // Get the data
            object data = dataObject.GetData(format, true);

            // If the data is a stream, we'll check to see if it
            // is stamped by us for custom marshaling
            if (data is Stream)
            {
                object data2 = ComTypes.ComDataObjectExtensions.GetManagedData((ComTypes.IDataObject)dataObject, format);
                if (data2 != null)
                    return data2;
            }

            return data;
        }
    }
}

#endregion // SwfDragDropLib\SwfDataObjectExtensions.cs

#region SwfDragDropLib\SwfDragDropLibExtensions.cs

namespace DragDropLib
{
    using System.Drawing;

    public static class SwfDragDropLibExtensions
    {
        /// <summary>
        /// Converts a System.Windows.Point value to a DragDropLib.Win32Point value.
        /// </summary>
        /// <param name="pt">Input value.</param>
        /// <returns>Converted value.</returns>
        public static Win32Point ToWin32Point(this Point pt)
        {
            Win32Point wpt = new Win32Point();
            wpt.x = pt.X;
            wpt.y = pt.Y;
            return wpt;
        }
    }
}


#endregion // SwfDragDropLib\SwfDragDropLibExtensions.cs

#region SwfDragDropLib\SwfDropTargetHelper.cs

namespace System.Windows.Forms
{
    using System.Collections.Generic;
    using DragDropLib;

    public static class DropTargetHelper
    {
        /// <summary>
        /// Internal instance of the DragDropHelper.
        /// </summary>
        private static readonly IDropTargetHelper s_instance = (IDropTargetHelper)new DragDropHelper();

        /// <summary>
        /// Internal cache of IDataObjects related to drop targets.
        /// </summary>
        private static readonly IDictionary<Control, IDataObject> s_dataContext = new Dictionary<Control, IDataObject>();

        static DropTargetHelper()
        {
        }

        /// <summary>
        /// Notifies the DragDropHelper that the specified Control received
        /// a DragEnter event.
        /// </summary>
        /// <param name="control">The Control the received the DragEnter event.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragEnter(Control control, IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect)
        {
            SwfDropTargetHelperExtensions.DragEnter(s_instance, control, data, cursorOffset, effect);
        }

        /// <summary>
        /// Sets the drop description of the IDataObject and then notifies the
        /// DragDropHelper that the specified Control received a DragEnter event.
        /// </summary>
        /// <param name="control">The Control the received the DragEnter event.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        /// <param name="descriptionMessage">The drop description message.</param>
        /// <param name="descriptionInsert">The drop description insert.</param>
        /// <remarks>
        /// Because the DragLeave event in SWF does not provide the IDataObject in the
        /// event args, this DragEnter override of the DropTargetHelper will cache a
        /// copy of the IDataObject based on the provided Control so that it may be
        /// cleared using the DragLeave override that takes a Control parameter. Note that
        /// if you use this override of DragEnter, you must call the DragLeave override
        /// that takes a Control parameter to avoid a possible memory leak. However, calling
        /// this method multiple times with the same Control parameter while not calling the
        /// DragLeave method will not leak memory.
        /// </remarks>
        public static void DragEnter(Control control, IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect, string descriptionMessage, string descriptionInsert)
        {
            data.SetDropDescription((DropImageType)effect, descriptionMessage, descriptionInsert);
            DragEnter(control, data, cursorOffset, effect);

            if (!s_dataContext.ContainsKey(control))
                s_dataContext.Add(control, data);
            else
                s_dataContext[control] = data;
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a DragOver event.
        /// </summary>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragOver(System.Drawing.Point cursorOffset, DragDropEffects effect)
        {
            SwfDropTargetHelperExtensions.DragOver(s_instance, cursorOffset, effect);
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a DragLeave event.
        /// </summary>
        public static void DragLeave()
        {
            s_instance.DragLeave();
        }

        /// <summary>
        /// Clears the drop description of the IDataObject previously associated to the
        /// provided control, then notifies the DragDropHelper that the current control
        /// received a DragLeave event.
        /// </summary>
        /// <remarks>
        /// Because the DragLeave event in SWF does not provide the IDataObject in the
        /// event args, this DragLeave override of the DropTargetHelper will lookup a
        /// cached copy of the IDataObject based on the provided Control and clear
        /// the drop description. Note that the underlying DragLeave call of the
        /// Shell IDropTargetHelper object keeps the current control cached, so the
        /// control passed to this method is only relevant to looking up the IDataObject
        /// on which to clear the drop description.
        /// </remarks>
        public static void DragLeave(Control control)
        {
            if (s_dataContext.ContainsKey(control))
            {
                s_dataContext[control].SetDropDescription(DropImageType.Invalid, null, null);
                s_dataContext.Remove(control);
            }

            DragLeave();
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a DragOver event.
        /// </summary>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void Drop(IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect)
        {
            // No need to clear the drop description, but don't keep it stored to avoid memory leaks
            foreach (KeyValuePair<Control, IDataObject> pair in s_dataContext)
            {
                if (object.ReferenceEquals(pair.Value, data))
                {
                    s_dataContext.Remove(pair);
                    break;
                }
            }

            SwfDropTargetHelperExtensions.Drop(s_instance, data, cursorOffset, effect);
        }

        /// <summary>
        /// Tells the DragDropHelper to show or hide the drag image.
        /// </summary>
        /// <param name="show">True to show the image. False to hide it.</param>
        public static void Show(bool show)
        {
            s_instance.Show(show);
        }
    }
}

#endregion // SwfDragDropLib\SwfDropTargetHelper.cs

#region SwfDragDropLib\SwfDropTargetHelperExtensions.cs

namespace DragDropLib
{
    using System;
    using System.Windows.Forms;
    using System.Drawing;
    using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

    public static class SwfDropTargetHelperExtensions
    {
        /// <summary>
        /// Notifies the DragDropHelper that the specified Control received
        /// a DragEnter event.
        /// </summary>
        /// <param name="dropHelper">The DragDropHelper instance to notify.</param>
        /// <param name="control">The Control the received the DragEnter event.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragEnter(this IDropTargetHelper dropHelper, Control control, IDataObject data, Point cursorOffset, DragDropEffects effect)
        {
            IntPtr controlHandle = IntPtr.Zero;
            if (control != null)
                controlHandle = control.Handle;
            Win32Point pt = SwfDragDropLibExtensions.ToWin32Point(cursorOffset);
            dropHelper.DragEnter(controlHandle, (ComIDataObject)data, ref pt, (int)effect);
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a DragOver event.
        /// </summary>
        /// <param name="dropHelper">The DragDropHelper instance to notify.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragOver(this IDropTargetHelper dropHelper, Point cursorOffset, DragDropEffects effect)
        {
            Win32Point pt = SwfDragDropLibExtensions.ToWin32Point(cursorOffset);
            dropHelper.DragOver(ref pt, (int)effect);
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Control received
        /// a Drop event.
        /// </summary>
        /// <param name="dropHelper">The DragDropHelper instance to notify.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void Drop(this IDropTargetHelper dropHelper, IDataObject data, Point cursorOffset, DragDropEffects effect)
        {
            Win32Point pt = SwfDragDropLibExtensions.ToWin32Point(cursorOffset);
            dropHelper.Drop((ComIDataObject)data, ref pt, (int)effect);
        }
    }
}

#endregion // SwfDragDropLib\SwfDropTargetHelperExtensions.cs
