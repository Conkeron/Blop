class BlopEngine {
	constructor(blop) {
		this.blop = blop;
		this.replaceFooter("engine version: gamejam");
	}
	replaceHeader(html) {
		const header = document.getElementsByTagName("header")[0];
		header.innerHTML = html ?? "";
	}
	replaceFooter(html) {
		const header = document.getElementsByTagName("footer")[0];
		header.innerHTML = html ?? "";
	}
	replaceMain(html) {
		const header = document.getElementsByTagName("main")[0];
		header.innerHTML = html ?? "";
	}
	replaceAside(html) {
		const header = document.getElementsByTagName("aside")[0];
		header.innerHTML = html ?? "";
	}

	setMainTitle() {
		document.title = this.blop.gameData.title;
		this.replaceHeader(`<h1>${this.blop.gameData.title}</h1><em>${this.blop.gameData.description}</em>`);
	}
	showMainMenu() {
		let html = "<h2>Levels</h2><ol>";
		this.blop.gameData.levels.forEach(level => {
			html += `<li><h3>${level.title}</h3><em>${level.description}</em></li>`
		});
		html += "</ol>";

		this.replaceMain(html);
		this.setMainTitle();
		this.replaceAside();
	}
}
class Blop {
	constructor() {
		this.gameData = JSON.parse(document.getElementById("game-data").innerText);
		this.engine = new BlopEngine(this);
		this.engine.showMainMenu();
	}
}
