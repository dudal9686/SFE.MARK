/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** AXS.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Motion Library Header File
** 
**
*****************************************************************************
*****************************************************************************
**
** Source Change IndicesAxmStatusGetCmdPos
** ---------------------
**
** (None)
**
*****************************************************************************
*****************************************************************************
**
** Website
** ---------------------
**
** http://www.ajinextek.com
**
*****************************************************************************
*****************************************************************************
*/
using System;
using System.Runtime.InteropServices;

public class CAXS
{
    //========== ���� �� ��� Ȯ���Լ�(Info) - Infomation ===============================================================
    // �ش� ���� �����ȣ, ��� ��ġ, ��� ���̵� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsInfoGetPort(int nAxisNo, ref int npBoardNo, ref int npModulePos, ref uint upModuleID);
	//������ ��� ��ȣ�� �ش� ����� Sub ID, ��� Name, ��� ������ Ȯ���Ѵ�.
    //======================================================/
    // ���� ��ǰ            : EtherCAT
    // upModuleSubID        : EtherCAT ����� �����ϱ� ���� SubID
    // szModuleName         : ����� �𵨸�(up to 50 Byte)
    // szModuleDescription  : ��⿡ ���� ����(up to 80 Byte)
    //======================================================//    
    [DllImport("AXL.dll")] public static extern uint AxsInfoGetPortEx(int nPortNo, ref uint upModuleSubID, System.Text.StringBuilder szModuleName, System.Text.StringBuilder szModuleDescription);

    // ��� ����� �����ϴ��� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsInfoIsSerialModule(ref uint upStatus);
    // �ش� ��Ʈ�� ��ȿ���� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsInfoIsInvalidPortNo(int nPortNo);
    // �ش� ��Ʈ�� ��� ������ �������� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsInfoGetPortStatus(int nPortNo);
    // �ý��۳� ��ȿ�� �����Ʈ���� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsInfoGetPortCount(ref int npPortCount);
    // �ش� ����/����� ù��° ���ȣ�� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsInfoGetFirstPortNo(int nBoardNo, int nModulePos, ref int npPortNo);
    // �ش� ������ ù��° �����Ʈ��ȣ�� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsInfoGetBoardFirstPortNo(int nBoardNo, int nModulePos, ref int lpPortNo);
    //========== �ø��� ����Լ�(Port) =================================================================================
    // �����Ʈ�� Open�Ѵ�. PortOpen�� �ϳ��� �������α׷������� �� �� ����.
    // lBaudRate : 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200
    // lDataBits : 7, 8 
    // lStopBits : 1, 2
    // lParity   : [0]None, [1]Even, [2]Odd
    // dwFlagsAndAttributes : Reserved
    [DllImport("AXL.dll")] public static extern uint AxsPortOpen(int nPortNo, int nBaudRate, int nDataBits, int nStopBits, int nParity, uint dwFlagsAndAttributes);
    // �����Ʈ�� Close�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxsPortClose(int nPortNo);
     // �����Ʈ�� �����Ѵ�.(��� ���۴� �ʱ�ȭ���� ����)
    // lpDCB->BaudRate  : 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200
    // lpDCB->ByteSize  : 7, 8  
    // lpDCB->StopBits  : 1, 2
    // lpDCB->Parity    : [0]None, [1]Even, [2]Odd    
    [DllImport("kernel32.dll")] public static extern uint AxsPortSetCommState(int nPortNo, ref DCB lpDCB);
    // �����Ʈ�� �������� Ȯ���Ѵ�.
    [DllImport("kernel32.dll")] public static extern uint AxsPortGetCommState(int nPortNo, ref DCB lpDCB);
    // �����Ʈ�� Ÿ�Ӿƿ����� �����Ѵ�.
    // lpCommTimeouts->ReadIntervalTimeout          : ���ڿ� �Է��� ���۵� �� ���ڿ��� Timeout �ð� ����(milliseconds)
    // lpCommTimeouts->ReadTotalTimeoutMultiplier;  : �б� ���۽� ������ ��żӵ����� �ϳ��� ���ڿ��� ���� Timeout�ð� ����(milliseconds)
    // lpCommTimeouts->ReadTotalTimeoutConstant;    : �Է¹��� ���ڼ��� ���� Timeout�� ������ Timeout�ð� ����(milliseconds)
    // lpCommTimeouts->WriteTotalTimeoutMultiplier; : ���� ���۽� ������ ��żӵ����� �ϳ��� ���ڿ��� ���� Timeout�ð� ����(milliseconds)
    // lpCommTimeouts->WriteTotalTimeoutConstant;   : ������ ���ڼ��� ���� Timeout�� ������ Timeout�ð� ����(milliseconds)
    [DllImport("kernel32.dll")] public static extern uint AxsPortSetCommTimeouts(int nPortNo, ref COMMTIMEOUTS lpCommTimeouts);
    // �����Ʈ�� Ÿ�Ӿƿ����� Ȯ���Ѵ�.
    [DllImport("kernel32.dll")] public static extern uint AxsPortGetCommTimeouts(int nPortNo, ref COMMTIMEOUTS lpCommTimeouts);
    // ��ġ�� ���� Flag�� ����ų� �ۼ��ŵ� ����Ÿ�� ������ Ȯ���Ѵ�.
    // lpErrors : 
    //      [1]CE_RXOVER:       ���Ź��� Overflow �߻�
    //      [2]CE_OVERRUN:      ���Ź��� Overrun �����߻�
    //      [4]CE_RXPARITY:     ���� ����Ÿ �и�Ƽ��Ʈ �����߻�
    //      [8]CE_FRAME:        ���� Framing �����߻�      
    // lpStat->cbInQue :        ���Ź��ۿ� �Էµ� ����Ÿ ����
    // lpStat->cbOutQue:        �۽Ź��ۿ� ���� ����Ÿ ����
    [DllImport("kernel32.dll")] public static extern uint AxsPortClearCommError(int nPortNo, out uint lpErrors, ref COMSTAT lpStat);
    // ����Ÿ �۽��� ����
    [DllImport("AXL.dll")] public static extern uint AxsPortSetCommBreak(int nPortNo);
    // ����Ÿ �۽��� �簳
    [DllImport("AXL.dll")] public static extern uint AxsPortClearCommBreak(int nPortNo);	    
    // �ۼ����� �����ϰų� ���۸� ����
    // dwFlags: 
    //      [1]PURGE_TXABORT:    ���� �۾��� ������
    //      [2]PURGE_RXABORT:    �б� �۾��� ������
    //      [4]PURGE_TXCLEAR:    �۽� ���ۿ� ����Ÿ�� ������� ����
    //      [8]PURGE_RXCLEAR:    ���� ���ۿ� ����Ÿ�� ������� ����
    [DllImport("AXL.dll")] public static extern uint AxsPortPurgeComm(int nPortNo, uint dwFlags);
    // �ø��� ��Ʈ�� ����Ÿ�� ��
    // lpBuffer :                ��ġ�� �� �����͸� ��� ������ ����Ʈ��
    // nNumberOfBytesToWrite :   lpBuffer�� ��� ���� �������� ����Ʈ ��
    // lpNumberOfBytesWritten :  ������ ������ ����Ʈ ���� ��ȯ(None Overrapped �ϰ��)
    // lpOverlapped :            �񵿱⸦ ���� OVERLAPPED ����ü�� ����Ʈ ��
    [DllImport("kernel32.dll")] public static extern uint AxsPortWriteFile(int nPortNo, IntPtr lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
    // �ø��� ��Ʈ�� ����Ÿ�� ����
    // lpBuffer :               ��ġ�� �� �����͸� ��� ������ ����Ʈ��
    // nNumberOfBytesToRead :   lpBuffer�� ����Ű�� ������ ũ�⸦ ����Ʈ�� ����
    // lpNumberOfBytesRead :    ������ ������ ����Ʈ ���� ��ȯ(None Overrapped �ϰ��)
    // lpOverlapped :           �񵿱⸦ ���� OVERLAPPED ����ü�� ����Ʈ ��
    [DllImport("kernel32.dll")] public static extern uint AxsPortReadFile(int nPortNo, IntPtr lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, ref OVERLAPPED lpOverlapped);

    // �ø��� ��Ʈ�� Overlapped �۾��� ����� ���� ��ȯ 
    // lpOverlapped->hEvent :       ������ �Ϸ�� �� �ñ׳ε� �̺�Ʈ �ڵ�. AxsPortWriteFile, AxsPortReadFile �Լ��� ����ϱ����� �� ���� ������.
    // lpNumberOfBytesTransferred:  ���� ������ �Ϸ�� ����Ʈ ũ�⸦ ��� ���� ��������Ʈ
    // bWait:                       I/O ������ ���������� ��Ȳ������ ó���� ����
    //      [0]: I/O������ ���������� ��ٸ�
    //      [1]: I/O������ ������ �ʾƵ� ��ȯ��
    [DllImport("AXL.dll")] public static extern uint AxsPortGetOverlappedResult(int nPortNo, ref OVERLAPPED lpOverlapped, out uint lpNumberOfBytesTransferred, bool bWait);
    
    // �ø��� ��Ʈ�� �߻��� ���� �����ڵ带 ��ȯ    
    // [  0]ERROR_SUCCESS          ��������
    // [  5]ERROR_ACCESS_DENIED    �����Ʈ�� ������� ���
    // [  6]ERROR_INVALID_HANDLE   �����Ʈ�� ��ȿ���� ���� ���(��Ʈ��ũ �����������)
    // [ 87]ERROR_INVALID_PARAMETER �߸��� �Ķ��Ÿ ����   
    // [995]ERROR_OPERATION_ABORTED The I/O operation has been aborted because of either a thread exit or an application request.
    // [996]ERROR_IO_INCOMPLETE     Overrapped����� �� �����۾��� ������ ������쳪 Timeout�� �߻��� ���
    // [997]ERROR_IO_PENDING        Overrapped����� �� I/O�۾��� �������� ���
    // [998]ERROR_NOACCESS
    [DllImport("AXL.dll")] public static extern uint AxsPortGetLastError(int nPortNo, ref uint dwpErrCode);
    [DllImport("AXL.dll")] public static extern uint AxsPortSetLastError(int nPortNo, uint dwErrCode);
    
}
