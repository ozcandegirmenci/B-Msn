using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bmsn.Client.UI
{
    /// <summary>
    /// A richtextbox control which provides custom featuers for messaging textbox
    /// </summary>
    internal class OleRichTextBox : RichTextBox
	{
        #region Types 

        private class RichEditOle
        {
            OleRichTextBox rtf;
            IRichEditOle _RichEditOle;

            public RichEditOle(OleRichTextBox richEdit)
            {
                rtf = richEdit;
            }

            private IRichEditOle IRichEditOle
            {
                get
                {
                    if (_RichEditOle == null)
                        _RichEditOle = SendMessage(rtf.Handle, EM_GETOLEINTERFACE, 0);

                    return _RichEditOle;
                }
            }

            public void InsertImageDataObject(ImageDataObject ido)
            {
                InsertImageDataObject(ido, rtf.SelectionStart);
            }

            public void InsertImageDataObject(ImageDataObject ido, int position)
            {
                if (ido == null)
                    return;

                ILockBytes pLockBytes;
                int sc = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out pLockBytes);

                IStorage pStorage;
                sc = StgCreateDocfileOnILockBytes(pLockBytes, (uint)(STGM.STGM_SHARE_EXCLUSIVE
                    | STGM.STGM_CREATE | STGM.STGM_READWRITE), 0, out pStorage);

                IOleClientSite pOleClientSite;
                IRichEditOle.GetClientSite(out pOleClientSite);

                Guid guid = Marshal.GenerateGuidForType(ido.GetType());

                Guid IID_IOleObject = new Guid("{00000112-0000-0000-C000-000000000046}");
                Guid IID_IDataObject = new Guid("{0000010e-0000-0000-C000-000000000046}");
                Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

                object pOleObject;

                int hr = OleCreateStaticFromData(ido, ref IID_IOleObject, (uint)OLERENDER.OLERENDER_FORMAT,
                    ref ido._Formatetc, pOleClientSite, pStorage, out pOleObject);

                if (pOleObject == null)
                    return;
                OleSetContainedObject(pOleObject, true);

                REOBJECT reoObject = new REOBJECT();

                reoObject.cp = position;

                reoObject.clsid = guid;
                reoObject.pstg = pStorage;
                reoObject.poleobj = Marshal.GetIUnknownForObject(pOleObject);
                reoObject.polesite = pOleClientSite;
                reoObject.dvAspect = (uint)(DVASPECT.DVASPECT_CONTENT);
                reoObject.dwFlags = (uint)(REOOBJECTFLAGS.REO_BELOWBASELINE);
                reoObject.dwUser = 0;

                this.IRichEditOle.InsertObject(reoObject);

                Marshal.ReleaseComObject(pLockBytes);
                Marshal.ReleaseComObject(pOleClientSite);
                Marshal.ReleaseComObject(pStorage);
                Marshal.ReleaseComObject(pOleObject);
            }

            public void UpdateObjects()
            {
                int count = IRichEditOle.GetObjectCount();

                for (int i = 0; i < count; i++)
                {
                    REOBJECT reoObject = new REOBJECT();

                    IRichEditOle.GetObject(i, reoObject, GETOBJECTOPTIONS.REO_GETOBJ_ALL_INTERFACES);

                    if (reoObject.dwUser == 1)
                    {
                        Point pt = this.rtf.GetPositionFromCharIndex(reoObject.cp);
                        Rectangle rect = new Rectangle(pt, reoObject.sizel);
                        rtf.Invalidate(rect, false);
                    }
                }
            }
        }

        internal class ImageDataObject : IDataObject
        {
            Bitmap _Bitmap;
            public FORMATETC _Formatetc;

            const uint S_OK = 0;
            const uint E_POINTER = 0x80004003;
            const uint E_NOTIMPL = 0x80004001;
            const uint E_FAIL = 0x80004005;

            public ImageDataObject()
            {
                _Bitmap = new Bitmap(16, 16);
                _Formatetc = new FORMATETC();
            }

            public uint GetData(ref FORMATETC formatetc, ref STGMEDIUM medium)
            {
                IntPtr hDst = _Bitmap.GetHbitmap();

                medium.tymed = (int)TYMED.TYMED_GDI;
                medium.unionmember = hDst;
                medium.pUnkForRelease = IntPtr.Zero;

                return S_OK;
            }

            public uint GetDataHere(ref FORMATETC formatetc, out STGMEDIUM medium)
            {
                medium = new STGMEDIUM();
                return E_NOTIMPL;
            }

            public uint QueryGetData(ref FORMATETC formatetc)
            {
                return E_NOTIMPL;
            }

            public uint GetCanonicalFormatEtc(ref FORMATETC formatetcIn, out FORMATETC formatetcOut)
            {
                formatetcOut = new FORMATETC();
                return E_NOTIMPL;
            }

            public uint SetData(ref FORMATETC a, ref STGMEDIUM b, bool fRelease)
            {
                return S_OK;
            }

            public uint EnumFormatEtc(uint dwDirection, IEnumFORMATETC penum)
            {
                return S_OK;
            }

            public uint DAdvise(ref FORMATETC a, int advf, IAdviseSink advSink, out uint connection)
            {
                connection = 0;
                return E_NOTIMPL;
            }

            public uint DUnadvise(uint connection)
            {
                return E_NOTIMPL;
            }

            public uint EnumDAdvise(out IEnumSTATDATA enumAdvise)
            {
                enumAdvise = null;
                return E_NOTIMPL;
            }

            public void SetImage(Image image)
            {
                try
                {
                    _Bitmap = new Bitmap(image);

                    _Formatetc.cfFormat = CLIPFORMAT.CF_BITMAP;
                    _Formatetc.ptd = IntPtr.Zero;
                    _Formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
                    _Formatetc.lindex = -1;
                    _Formatetc.tymed = TYMED.TYMED_GDI;
                }
                catch
                {
                }
            }
        }

        [Flags(), ComVisible(false)]
        public enum STGM : int
        {
            STGM_DIRECT = 0x0,
            STGM_TRANSACTED = 0x10000,
            STGM_SIMPLE = 0x8000000,
            STGM_READ = 0x0,
            STGM_WRITE = 0x1,
            STGM_READWRITE = 0x2,
            STGM_SHARE_DENY_NONE = 0x40,
            STGM_SHARE_DENY_READ = 0x30,
            STGM_SHARE_DENY_WRITE = 0x20,
            STGM_SHARE_EXCLUSIVE = 0x10,
            STGM_PRIORITY = 0x40000,
            STGM_DELETEONRELEASE = 0x4000000,
            STGM_NOSCRATCH = 0x100000,
            STGM_CREATE = 0x1000,
            STGM_CONVERT = 0x20000,
            STGM_FAILIFTHERE = 0x0,
            STGM_NOSNAPSHOT = 0x200000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLBARINFO
        {
            public int cbSize;
            public RECT rcScrollBar;
            public int dxyLineButton;
            public int xyThumbTop;
            public int xyThumbBottom;
            public int reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] rgstate;
        }

        [Flags(), ComVisible(false)]
        public enum DVASPECT : int
        {
            DVASPECT_CONTENT = 1,
            DVASPECT_THUMBNAIL = 2,
            DVASPECT_ICON = 4,
            DVASPECT_DOCPRINT = 8,
            DVASPECT_OPAQUE = 16,
            DVASPECT_TRANSPARENT = 32,
        }

        [ComVisible(false)]
        public enum CLIPFORMAT : int
        {
            CF_TEXT = 1,
            CF_BITMAP = 2,
            CF_METAFILEPICT = 3,
            CF_SYLK = 4,
            CF_DIF = 5,
            CF_TIFF = 6,
            CF_OEMTEXT = 7,
            CF_DIB = 8,
            CF_PALETTE = 9,
            CF_PENDATA = 10,
            CF_RIFF = 11,
            CF_WAVE = 12,
            CF_UNICODETEXT = 13,
            CF_ENHMETAFILE = 14,
            CF_HDROP = 15,
            CF_LOCALE = 16,
            CF_MAX = 17,
            CF_OWNERDISPLAY = 0x80,
            CF_DSPTEXT = 0x81,
            CF_DSPBITMAP = 0x82,
            CF_DSPMETAFILEPICT = 0x83,
            CF_DSPENHMETAFILE = 0x8E,
        }

        [Flags(), ComVisible(false)]
        public enum REOOBJECTFLAGS : uint
        {
            REO_NULL = 0x00000000,
            REO_READWRITEMASK = 0x0000003F,
            REO_DONTNEEDPALETTE = 0x00000020,
            REO_BLANK = 0x00000010,
            REO_DYNAMICSIZE = 0x00000008,
            REO_INVERTEDSELECT = 0x00000004,
            REO_BELOWBASELINE = 0x00000002,
            REO_RESIZABLE = 0x00000001,
            REO_LINK = 0x80000000,
            REO_STATIC = 0x40000000,
            REO_SELECTED = 0x08000000,
            REO_OPEN = 0x04000000,
            REO_INPLACEACTIVE = 0x02000000,
            REO_HILITED = 0x01000000,
            REO_LINKAVAILABLE = 0x00800000,
            REO_GETMETAFILE = 0x00400000
        }

        [ComVisible(false)]
        public enum OLERENDER : int
        {
            OLERENDER_NONE = 0,
            OLERENDER_DRAW = 1,
            OLERENDER_FORMAT = 2,
            OLERENDER_ASIS = 3,
        }

        [Flags, ComVisible(false)]
        public enum TYMED : int
        {
            TYMED_NULL = 0,
            TYMED_HGLOBAL = 1,
            TYMED_FILE = 2,
            TYMED_ISTREAM = 4,
            TYMED_ISTORAGE = 8,
            TYMED_GDI = 16,
            TYMED_MFPICT = 32,
            TYMED_ENHMF = 64,
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct FORMATETC
        {
            public CLIPFORMAT cfFormat;
            public IntPtr ptd;
            public DVASPECT dwAspect;
            public int lindex;
            public TYMED tymed;
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct STGMEDIUM
        {
            //[MarshalAs(UnmanagedType.I4)]
            public int tymed;
            public IntPtr unionmember;
            public IntPtr pUnkForRelease;
        }

        [ComVisible(true),
        ComImport(),
        Guid("00000103-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumFORMATETC
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Next(
                [In, MarshalAs(UnmanagedType.U4)]
            int celt,
                [Out]
            FORMATETC rgelt,
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
            int[] pceltFetched);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Skip(
                [In, MarshalAs(UnmanagedType.U4)]
            int celt);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Reset();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Clone(
                [Out, MarshalAs(UnmanagedType.LPArray)]
            IEnumFORMATETC[] ppenum);
        }

        [StructLayout(LayoutKind.Sequential),
        ComVisible(true)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width
            {
                get { return right - left; }
            }

            public int Height
            {
                get { return bottom - top; }
            }
        }

        public enum GETOBJECTOPTIONS
        {
            REO_GETOBJ_NO_INTERFACES = 0x00000000,
            REO_GETOBJ_POLEOBJ = 0x00000001,
            REO_GETOBJ_PSTG = 0x00000002,
            REO_GETOBJ_POLESITE = 0x00000004,
            REO_GETOBJ_ALL_INTERFACES = 0x00000007,
        }

        public enum GETCLIPBOARDDATAFLAGS
        {
            RECO_PASTE = 0,
            RECO_DROP = 1,
            RECO_COPY = 2,
            RECO_CUT = 3,
            RECO_DRAG = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class REOBJECT
        {
            public int cbStruct = Marshal.SizeOf(typeof(REOBJECT));
            public int cp;
            public Guid clsid;
            public IntPtr poleobj;
            public IStorage pstg;
            public IOleClientSite polesite;
            public Size sizel;
            public uint dvAspect;
            public uint dwFlags;
            public uint dwUser;
        }

        [ComVisible(true), Guid("0000010F-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAdviseSink
        {
            void OnDataChange(
                [In]
            FORMATETC pFormatetc,
                [In]
            STGMEDIUM pStgmed);

            void OnViewChange(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwAspect,
                [In, MarshalAs(UnmanagedType.I4)]
            int lindex);

            void OnRename(
                [In, MarshalAs(UnmanagedType.Interface)]
            object pmk);

            void OnSave();

            void OnClose();
        }

        [ComVisible(false), StructLayout(LayoutKind.Sequential)]
        public sealed class STATDATA
        {
            [MarshalAs(UnmanagedType.U4)]
            public int advf;

            [MarshalAs(UnmanagedType.U4)]
            public int dwConnection;
        }

        [ComVisible(false), StructLayout(LayoutKind.Sequential)]
        public sealed class tagOLEVERB
        {
            [MarshalAs(UnmanagedType.I4)]
            public int lVerb;

            [MarshalAs(UnmanagedType.LPWStr)]
            public String lpszVerbName;

            [MarshalAs(UnmanagedType.U4)]
            public int fuFlags;

            [MarshalAs(UnmanagedType.U4)]
            public int grfAttribs;

        }

        [ComVisible(true), ComImport(),
        Guid("00000104-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumOLEVERB
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Next(
                [MarshalAs(UnmanagedType.U4)]
            int celt,
                [Out]
            tagOLEVERB rgelt,
                [Out, MarshalAs(UnmanagedType.LPArray)]
            int[] pceltFetched);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Skip(
                [In, MarshalAs(UnmanagedType.U4)]
            int celt);

            void Reset();

            void Clone(
                out IEnumOLEVERB ppenum);
        }

        [ComVisible(true),
        Guid("00000105-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumSTATDATA
        {
            void Next(
                [In, MarshalAs(UnmanagedType.U4)]
            int celt,
                [Out]
            STATDATA rgelt,
                [Out, MarshalAs(UnmanagedType.LPArray)]
            int[] pceltFetched);

            void Skip(
                [In, MarshalAs(UnmanagedType.U4)]
            int celt);

            void Reset();

            void Clone(
                [Out, MarshalAs(UnmanagedType.LPArray)]
            IEnumSTATDATA[] ppenum);
        }

        [ComVisible(true),
        Guid("0000011B-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleContainer
        {

            void ParseDisplayName(
                [In, MarshalAs(UnmanagedType.Interface)]
            object pbc,
                [In, MarshalAs(UnmanagedType.BStr)]
            string pszDisplayName,
                [Out, MarshalAs(UnmanagedType.LPArray)]
            int[] pchEaten,
                [Out, MarshalAs(UnmanagedType.LPArray)]
            object[] ppmkOut);

            void EnumObjects(
                [In, MarshalAs(UnmanagedType.U4)]
            int grfFlags,
                [Out, MarshalAs(UnmanagedType.LPArray)]
            object[] ppenum);

            void LockContainer(
                [In, MarshalAs(UnmanagedType.I4)]
            int fLock);
        }

        [ComVisible(true),
        ComImport(),
        Guid("0000010E-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDataObject
        {
            [PreserveSig()]
            uint GetData(
                ref FORMATETC a,
                ref STGMEDIUM b);

            [PreserveSig()]
            uint GetDataHere(
                ref FORMATETC pFormatetc,
                out STGMEDIUM pMedium);

            [PreserveSig()]
            uint QueryGetData(
                ref FORMATETC pFormatetc);

            [PreserveSig()]
            uint GetCanonicalFormatEtc(
                ref FORMATETC pformatectIn,
                out FORMATETC pformatetcOut);

            [PreserveSig()]
            uint SetData(
                ref FORMATETC pFormatectIn,
                ref STGMEDIUM pmedium,
                [In, MarshalAs(UnmanagedType.Bool)]
            bool fRelease);

            [PreserveSig()]
            uint EnumFormatEtc(
                uint dwDirection, IEnumFORMATETC penum);

            [PreserveSig()]
            uint DAdvise(
                ref FORMATETC pFormatetc,
                int advf,
                [In, MarshalAs(UnmanagedType.Interface)]
            IAdviseSink pAdvSink,
                out uint pdwConnection);

            [PreserveSig()]
            uint DUnadvise(
                uint dwConnection);

            [PreserveSig()]
            uint EnumDAdvise(
                [Out, MarshalAs(UnmanagedType.Interface)]
            out IEnumSTATDATA ppenumAdvise);
        }

        [ComVisible(true), Guid("00000118-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleClientSite
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SaveObject();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetMoniker(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwAssign,
                [In, MarshalAs(UnmanagedType.U4)]
            int dwWhichMoniker,
                [Out, MarshalAs(UnmanagedType.Interface)]
            out object ppmk);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetContainer([MarshalAs(UnmanagedType.Interface)] out IOleContainer container);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ShowObject();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnShowWindow(
                [In, MarshalAs(UnmanagedType.I4)] int fShow);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int RequestNewObjectLayout();
        }

        [ComVisible(false), StructLayout(LayoutKind.Sequential)]
        public sealed class tagLOGPALETTE
        {
            [MarshalAs(UnmanagedType.U2)]
            public short palVersion;

            [MarshalAs(UnmanagedType.U2)]
            public short palNumEntries;

        }

        [ComVisible(true), ComImport(),
        Guid("00000112-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleObject
        {

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetClientSite(
                [In, MarshalAs(UnmanagedType.Interface)]
            IOleClientSite pClientSite);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetClientSite(out IOleClientSite site);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetHostNames(
                [In, MarshalAs(UnmanagedType.LPWStr)]
            string szContainerApp,
                [In, MarshalAs(UnmanagedType.LPWStr)]
            string szContainerObj);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Close(
                [In, MarshalAs(UnmanagedType.I4)]
            int dwSaveOption);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetMoniker(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwWhichMoniker,
                [In, MarshalAs(UnmanagedType.Interface)]
            object pmk);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetMoniker(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwAssign,
                [In, MarshalAs(UnmanagedType.U4)]
            int dwWhichMoniker,
                out object moniker);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int InitFromData(
                [In, MarshalAs(UnmanagedType.Interface)]
            IDataObject pDataObject,
                [In, MarshalAs(UnmanagedType.I4)]
            int fCreation,
                [In, MarshalAs(UnmanagedType.U4)]
            int dwReserved);

            int GetClipboardData(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwReserved,
                out IDataObject data);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int DoVerb(
                [In, MarshalAs(UnmanagedType.I4)]
            int iVerb,
                [In]
            IntPtr lpmsg,
                [In, MarshalAs(UnmanagedType.Interface)]
            IOleClientSite pActiveSite,
                [In, MarshalAs(UnmanagedType.I4)]
            int lindex,
                [In]
            IntPtr hwndParent,
                [In]
            RECT lprcPosRect);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int EnumVerbs(out IEnumOLEVERB e);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Update();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int IsUpToDate();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetUserClassID(
                [In, Out]
            ref Guid pClsid);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetUserType(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwFormOfType,
                [Out, MarshalAs(UnmanagedType.LPWStr)]
            out string userType);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetExtent(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwDrawAspect,
                [In]
            Size pSizel);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetExtent(
                [In, MarshalAs(UnmanagedType.U4)]
            int dwDrawAspect,
                [Out]
            Size pSizel);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Advise([In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink, out int cookie);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int EnumAdvise(out IEnumSTATDATA e);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetColorScheme([In] tagLOGPALETTE pLogpal);
        }

        [ComImport]
        [Guid("0000000d-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumSTATSTG
        {
            [PreserveSig]
            uint Next(
                uint celt,
                [MarshalAs(UnmanagedType.LPArray), Out]
            STATSTG[] rgelt,
                out uint pceltFetched
                );

            void Skip(uint celt);

            void Reset();

            [return: MarshalAs(UnmanagedType.Interface)]
            IEnumSTATSTG Clone();
        }

        [ComImport]
        [Guid("0000000b-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IStorage
        {
            int CreateStream(
                string pwcsName,
                uint grfMode,
                uint reserved1,
                uint reserved2,
                out IStream ppstm);

            int OpenStream(
                string pwcsName,
                IntPtr reserved1,
                uint grfMode,
                uint reserved2,
                out IStream ppstm);

            int CreateStorage(
                string pwcsName,
                uint grfMode,
                uint reserved1,
                uint reserved2,
                out IStorage ppstg);

            int OpenStorage(
                string pwcsName,
                IStorage pstgPriority,
                uint grfMode,
                IntPtr snbExclude,
                uint reserved,
                out IStorage ppstg);

            int CopyTo(
                uint ciidExclude,
                Guid rgiidExclude,
                IntPtr snbExclude,
                IStorage pstgDest);

            int MoveElementTo(
                string pwcsName,
                IStorage pstgDest,
                string pwcsNewName,
                uint grfFlags);

            int Commit(
                uint grfCommitFlags);

            int Revert();

            int EnumElements(
                uint reserved1,
                IntPtr reserved2,
                uint reserved3,
                out IEnumSTATSTG ppenum);

            int DestroyElement(
                string pwcsName);

            int RenameElement(
                string pwcsOldName,
                string pwcsNewName);

            int SetElementTimes(
                string pwcsName,
                FILETIME pctime,
                FILETIME patime,
                FILETIME pmtime);

            int SetClass(
                Guid clsid);

            int SetStateBits(
                uint grfStateBits,
                uint grfMask);

            int Stat(
                out STATSTG pstatstg,
                uint grfStatFlag);

        }

        [ComImport]
        [Guid("0000000a-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ILockBytes
        {
            int ReadAt(
                ulong ulOffset,
                IntPtr pv,
                uint cb,
                out IntPtr pcbRead);

            int WriteAt(
                ulong ulOffset,
                IntPtr pv,
                uint cb,
                out IntPtr pcbWritten);

            int Flush();

            int SetSize(
                ulong cb);

            int LockRegion(
                ulong libOffset,
                ulong cb,
                uint dwLockType);

            int UnlockRegion(
                ulong libOffset,
                ulong cb,
                uint dwLockType);

            int Stat(
                out STATSTG pstatstg,
                uint grfStatFlag);

        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("0c733a30-2a1c-11ce-ade5-00aa0044773d")]
        public interface ISequentialStream
        {
            int Read(
                IntPtr pv,
                uint cb,
                out uint pcbRead);

            int Write(
                IntPtr pv,
                uint cb,
                out uint pcbWritten);

        };

        [ComImport]
        [Guid("0000000c-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IStream : ISequentialStream
        {
            int Seek(ulong dlibMove, uint dwOrigin, out ulong plibNewPosition);

            int SetSize(ulong libNewSize);

            int CopyTo([In] IStream pstm, ulong cb, out ulong pcbRead, out ulong pcbWritten);

            int Commit(uint grfCommitFlags);

            int Revert();

            int LockRegion(ulong libOffset, ulong cb, uint dwLockType);

            int UnlockRegion(ulong libOffset, ulong cb, uint dwLockType);

            int Stat(out STATSTG pstatstg, uint grfStatFlag);

            int Clone(out IStream ppstm);

        }

        [ComImport, Guid("00020D00-0000-0000-c000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRichEditOle
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetClientSite(out IOleClientSite site);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetObjectCount();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetLinkCount();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetObject(int iob, [In, Out] REOBJECT lpreobject, [MarshalAs(UnmanagedType.U4)]GETOBJECTOPTIONS flags);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int InsertObject(REOBJECT lpreobject);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ConvertObject(int iob, Guid rclsidNew, string lpstrUserTypeNew);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ActivateAs(Guid rclsid, Guid rclsidAs);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetHostNames(string lpstrContainerApp, string lpstrContainerObj);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetLinkAvailable(int iob, bool fAvailable);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetDvaspect(int iob, uint dvaspect);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int HandsOffStorage(int iob);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SaveCompleted(int iob, IStorage lpstg);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int InPlaceDeactivate();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ContextSensitiveHelp(bool fEnterMode);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetClipboardData([In, Out] ref CHARRANGE lpchrg, [MarshalAs(UnmanagedType.U4)] GETCLIPBOARDDATAFLAGS reco, out IDataObject lplpdataobj);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ImportDataObject(IDataObject lpdataobj, int cf, IntPtr hMetaPict);
        }

        #endregion

        #region Members

        public const int SC_VSCROLL = 0xF070;
        public const int SC_HSCROLL = 0xF080;
        public const int WM_PAINT = 0x000F;
        public const int WM_USER = 0x400;
        public const int EM_SETRECT = 0x00B3;
        public const int EM_SETTYPOGRAPHYOPTIONS = 0x04CA;
        public const int EM_GETOLEINTERFACE = WM_USER + 60;
        public const int TO_ADVANCEDTYPOGRAPHY = 0x01;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the border style of the control
        /// </summary>
        /// <remarks>
        /// Overrides the default value of the property
        /// </remarks>
        [DefaultValue(BorderStyle.None)]
        public new BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set { base.BorderStyle = value; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public OleRichTextBox()
		{
			CheckTheme();
			base.BorderStyle = BorderStyle.None;
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts given image at the cursor position
        /// </summary>
        /// <param name="image"></param>
        public void InsertImage(Image image)
        {
            ImageDataObject ido = new ImageDataObject();
            ido.SetImage(image);
            InsertImageDataObject(ido);
        }

        /// <summary>
        /// Inserts image at the given position
        /// </summary>
        /// <param name="image"></param>
        /// <param name="position"></param>
        public void InsertImage(Image image, int position)
        {
            ImageDataObject ido = new ImageDataObject();
            ido.SetImage(image);
            InsertImageDataObject(ido, position);
        }

        /// <summary>
        /// Inserts image data object at the current cursor position
        /// </summary>
        /// <param name="ido"></param>
        public void InsertImageDataObject(ImageDataObject ido)
        {
            RichEditOle ole = new RichEditOle(this);
            ole.InsertImageDataObject(ido);
        }

        /// <summary>
        /// Inserts image data object at the given position
        /// </summary>
        /// <param name="ido"></param>
        /// <param name="position"></param>
        public void InsertImageDataObject(ImageDataObject ido, int position)
        {
            RichEditOle ole = new RichEditOle(this);
            ole.InsertImageDataObject(ido, position);
        }

        /// <summary>
        /// Update objects
        /// </summary>
        public void UpdateObjects()
        {
            RichEditOle ole = new RichEditOle(this);
            ole.UpdateObjects();
        }

        /// <summary>
        /// Sets formatting client rectangle of the control
        /// </summary>
        public void SetFormattingRectangle()
        {
            if (!base.IsHandleCreated)
                return;

            RECT rect = new RECT();
            rect.left = 2;
            rect.right = Width - 5;
            rect.top = 2;
            rect.bottom = Height - 3;
            SCROLLBARINFO info = new SCROLLBARINFO();
            bool val;
            switch (ScrollBars)
            {
                case RichTextBoxScrollBars.None:
                    break;
                case RichTextBoxScrollBars.Both:
                    info.cbSize = Marshal.SizeOf(info);
                    val = GetScrollBarInfo(Handle, SC_HSCROLL, ref info);
                    if (val && info.rcScrollBar.Height != 0)
                        rect.bottom -= SystemInformation.HorizontalScrollBarHeight;
                    val = GetScrollBarInfo(Handle, SC_VSCROLL, ref info);
                    if (val && info.rcScrollBar.Width != 0)
                        rect.right -= SystemInformation.VerticalScrollBarWidth;
                    break;
                case RichTextBoxScrollBars.ForcedBoth:
                    rect.bottom -= SystemInformation.HorizontalScrollBarHeight;
                    rect.right -= SystemInformation.VerticalScrollBarWidth;
                    break;
                case RichTextBoxScrollBars.ForcedHorizontal:
                    rect.bottom -= SystemInformation.HorizontalScrollBarHeight;
                    info.cbSize = Marshal.SizeOf(info);
                    val = GetScrollBarInfo(Handle, SC_VSCROLL, ref info);
                    if (val && info.rcScrollBar.Width != 0)
                        rect.right -= SystemInformation.VerticalScrollBarWidth;
                    break;
                case RichTextBoxScrollBars.ForcedVertical:
                    rect.right -= SystemInformation.VerticalScrollBarWidth;
                    info.cbSize = Marshal.SizeOf(info);
                    val = GetScrollBarInfo(Handle, SC_HSCROLL, ref info);
                    if (val && info.rcScrollBar.Height != 0)
                        rect.bottom -= SystemInformation.HorizontalScrollBarHeight;
                    break;
                case RichTextBoxScrollBars.Horizontal:
                    info.cbSize = Marshal.SizeOf(info);
                    val = GetScrollBarInfo(Handle, SC_HSCROLL, ref info);
                    if (val && info.rcScrollBar.Height != 0)
                        rect.bottom -= SystemInformation.HorizontalScrollBarHeight;
                    break;
                case RichTextBoxScrollBars.Vertical:
                    info.cbSize = Marshal.SizeOf(info);
                    val = GetScrollBarInfo(Handle, SC_VSCROLL, ref info);
                    if (val && info.rcScrollBar.Width != 0)
                        rect.right -= SystemInformation.VerticalScrollBarWidth;
                    break;
            }

            int hr = SendMessage(Handle, EM_SETRECT,
                0, ref rect);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
        }

        #endregion

        #region Protected Methods

        protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.V)
			{
				e.Handled = true;
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 22)
			{
				e.Handled = true;
				if (Clipboard.ContainsText(TextDataFormat.Text))
				{
					string text = Clipboard.GetData(DataFormats.Text) as string;
					if (text != null)
					{
						SelectedText = text;
					}
				}
			}
			base.OnKeyPress(e);
		}

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                DrawControlBorder();
                m.Result = IntPtr.Zero;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetFormattingRectangle();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SendMessage(Handle,
                EM_SETTYPOGRAPHYOPTIONS,
                TO_ADVANCEDTYPOGRAPHY,
                TO_ADVANCEDTYPOGRAPHY);
            SetFormattingRectangle();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks and sets the theme
        /// </summary>
        private void CheckTheme()
		{
			if ((System.Environment.OSVersion.Version.Major > 5) 
				|| ((System.Environment.OSVersion.Version.Major == 5)
						&& (System.Environment.OSVersion.Version.Minor >= 1)))
			{
				SetWindowTheme(Handle, " ", " ");
			}
		}

        /// <summary>
        /// Draw control border
        /// </summary>
        private void DrawControlBorder()
		{
			using (Graphics graphics = Graphics.FromHwnd(Handle))
			{
				Rectangle rct = ClientRectangle;
				rct.Width--;
				rct.Height--;

				Pen pen = Pens.DarkGray;
				if (!Enabled)
					pen = Pens.Gray;

				graphics.DrawRectangle(pen, rct);
			}
		}

        #endregion

        #region Imports

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetScrollBarInfo(IntPtr hWnd, int idObject, ref SCROLLBARINFO psbi);

		[DllImport("user32", CharSet = CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam,
			int lParam);

		[DllImport("user32", CharSet = CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam,
			ref RECT lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto, PreserveSig = false)]
		public static extern IRichEditOle SendMessage(IntPtr hWnd, int message, int wParam);

		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public static extern int SetWindowTheme(
			IntPtr hWnd,
			[MarshalAs(UnmanagedType.LPWStr)] 
			string pszSubAppName,
			[MarshalAs(UnmanagedType.LPWStr)] 
			string pszSubIdList);

		[DllImport("ole32.dll")]
		public static extern int OleSetContainedObject([MarshalAs(UnmanagedType.IUnknown)]
			object pUnk, bool fContained);

		[DllImport("ole32.dll")]
		public static extern int OleCreateFromFile([In] ref Guid rclsid,
			[MarshalAs(UnmanagedType.LPWStr)] string lpszFileName, [In] ref Guid riid,
			uint renderopt, ref FORMATETC pFormatEtc, IOleClientSite pClientSite,
			IStorage pStg, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		[DllImport("ole32.dll")]
		public static extern int OleCreateStaticFromData([MarshalAs(UnmanagedType.Interface)]IDataObject pSrcDataObj,
			[In] ref Guid riid, uint renderopt, ref FORMATETC pFormatEtc,
			IOleClientSite pClientSite, IStorage pStg,
			[MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		[DllImport("ole32.dll",
			 EntryPoint = "CreateILockBytesOnHGlobal",
			 ExactSpelling = true, PreserveSig = true, CharSet = CharSet.Ansi,
			 CallingConvention = CallingConvention.StdCall)]
		public static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease,
			[MarshalAs(UnmanagedType.Interface)]out ILockBytes ppLkbyt);

		[DllImport("ole32.dll")]
		public static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, uint grfMode,
			uint reserved, out IStorage ppstgOpen);

        #endregion
    }
}
