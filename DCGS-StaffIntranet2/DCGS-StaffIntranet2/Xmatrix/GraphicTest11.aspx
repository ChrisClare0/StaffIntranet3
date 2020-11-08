<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GraphicTest11.aspx.cs" Inherits="DCGS_Staff_Intranet2.Xmatrix.GraphicTest11" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<script src="../Scripts/Chart.js"></script>
<head runat="server">
    <title></title>
    <style>
         .canvas-holder {
    background-color: #FFFFFF;
    position: absolute;
    width: 400px;
    height: 400px;
 }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div  id="div1"  >
       
<canvas id="myChart" class="canvas-holder" ></canvas>

    </div>
<script>
var ctx = document.getElementById('myChart').getContext('2d');
var myChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [{
            label: 'THIS IS THE LABEL',
            data: [12, 19, 3, 5, 2, 3],
            backgroundColor: [
                'rgba(255, 0, 0, 0.1)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)'
            ],
            borderColor: [
                'rgba(255, 0, 0, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)'
            ],
            borderWidth: 2
        }]
    },
    options: {responsive:false ,   
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        }

    }
    
});
</script>

    </form>
</body>
</html>
