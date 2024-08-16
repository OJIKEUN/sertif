// Morris Days
var day_data = [
	{"period": "2016-10-01", "licensed": 3213, "Limit Less": 887},
	{"period": "2016-09-30", "licensed": 3321, "Limit Less": 776},
	{"period": "2016-09-29", "licensed": 3671, "Limit Less": 884},
	{"period": "2016-09-20", "licensed": 3176, "Limit Less": 448},
	{"period": "2016-09-19", "licensed": 3376, "Limit Less": 565},
	{"period": "2016-09-18", "licensed": 3976, "Limit Less": 627},
	{"period": "2016-09-17", "licensed": 2239, "Limit Less": 660},
	{"period": "2016-09-16", "licensed": 3871, "Limit Less": 676},
	{"period": "2016-09-15", "licensed": 3659, "Limit Less": 656},
	{"period": "2016-09-10", "licensed": 3380, "Limit Less": 663}
];
Morris.Line({
	element: 'dayData',
	data: day_data,
	xkey: 'period',
	ykeys: ['licensed', 'Limit Less'],
	labels: ['Licensed', 'Limit Less'],
	resize: true,
	hideHover: "auto",
	gridLineColor: "#e1e5f1",
	pointFillColors:['#ffffff'],
	pointStrokeColors: ['#ee0000'],
	lineColors:['#225de4', '#1146bf', '#edf3ff'],
});