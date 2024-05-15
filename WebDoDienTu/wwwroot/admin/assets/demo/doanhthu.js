// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#292b2c';

// Tính tổng doanh thu theo tháng
function calculateRevenueByMonth(data) {
  const revenueByMonth = {};
  data.forEach(entry => {
      const date = new Date(entry.date);
      const month = date.getMonth() + 1; // Tháng bắt đầu từ 0 nên cần cộng thêm 1
      const revenue = entry.amount;

      if (!revenueByMonth[month]) {
          revenueByMonth[month] = revenue;
      } else {
          revenueByMonth[month] += revenue;
      }
  });
  return revenueByMonth;
}


// Dữ liệu doanh thu từ CSDL (mô phỏng)
//const revenueData = [
//  { date: "2024-01-01", amount: 10000 },
//  { date: "2024-01-05", amount: 10000 },
//  { date: "2024-02-01", amount: 30162 },
//  { date: "2024-03-01", amount: 26263 },
//  { date: "2024-04-01", amount: 18394 },
//  { date: "2024-05-01", amount: 18287 },
//  { date: "2024-06-01", amount: 28682 },
//  { date: "2024-07-01", amount: 31274 },
//  { date: "2024-08-01", amount: 33259 },
//  { date: "2024-09-01", amount: 25849 },
//  { date: "2024-10-01", amount: 24159 },
//  { date: "2024-11-01", amount: 32651 },
//  { date: "2024-12-01", amount: 31984 },
//];

// Tính toán doanh thu theo tháng
const revenueByMonth = calculateRevenueByMonth(revenueData);

// Area Chart Example
var ctx = document.getElementById("myAreaChart");
var myLineChart = new Chart(ctx, {
  type: 'line',
  data: {
    labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"],
    datasets: [{
      label: "Doanh thu",
      lineTension: 0.3,
      backgroundColor: "rgba(2,117,216,0.2)",
      borderColor: "rgba(2,117,216,1)",
      pointRadius: 5,
      pointBackgroundColor: "rgba(2,117,216,1)",
      pointBorderColor: "rgba(255,255,255,0.8)",
      pointHoverRadius: 5,
      pointHoverBackgroundColor: "rgba(2,117,216,1)",
      pointHitRadius: 50,
      pointBorderWidth: 2,
      // data: [10000, 30162, 26263, 18394, 18287, 28682, 31274, 33259, 25849, 24159, 32651, 31984, 38451],
      data: [
        revenueByMonth[1] || 0, // Tháng 1
        revenueByMonth[2] || 0, // Tháng 2
        revenueByMonth[3] || 0, // Tháng 3
        revenueByMonth[4] || 0, // Tháng 4
        revenueByMonth[5] || 0, // Tháng 5
        revenueByMonth[6] || 0, // Tháng 6
        revenueByMonth[7] || 0, // Tháng 7
        revenueByMonth[8] || 0, // Tháng 8
        revenueByMonth[9] || 0, // Tháng 9
        revenueByMonth[10] || 0, // Tháng 10
        revenueByMonth[11] || 0, // Tháng 11
        revenueByMonth[12] || 0, // Tháng 12
        revenueByMonth[13] || 0, // Tháng 13
      ],
    }],
  },
  options: {
    scales: {
      xAxes: [{
        time: {
          unit: 'date'
        },
        gridLines: {
          display: false
        },
        ticks: {
          maxTicksLimit: 7
        }
      }],
      yAxes: [{
        ticks: {
          min: 0,
          max: 40000,
          maxTicksLimit: 5
        },
        gridLines: {
          color: "rgba(0, 0, 0, .125)",
        }
      }],
    },
    legend: {
      display: false
    }
  }
});
