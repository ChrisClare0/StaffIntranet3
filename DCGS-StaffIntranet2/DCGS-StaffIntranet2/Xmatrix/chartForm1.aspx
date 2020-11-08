<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="chartForm1.aspx.cs" Inherits="DCGS_Staff_Intranet2.Xmatrix.chartForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Chart ID="Chart8" runat="server" Width="1563px" Height="707px">
        <Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1"  Area3DStyle-Enable3D="false">
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
    </div>
    </form>
</body>
</html>
