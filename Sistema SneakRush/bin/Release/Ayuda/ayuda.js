/* SneakRush - Ayuda del sistema. Sin dependencias: funciona abriendo el HTML desde el disco. */
(function () {
    "use strict";

    var AYUDA = window.AYUDA || { secciones: [] };
    var actual = document.body.getAttribute("data-tema") || "";

    function norm(t) {
        return (t || "").toLowerCase().normalize("NFD").replace(/[̀-ͯ]/g, "");
    }
    function esc(t) {
        return String(t).replace(/[&<>"]/g, function (c) {
            return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;" }[c];
        });
    }

    /* ---------- Menu lateral ---------- */
    var lateral = document.getElementById("lateral");
    if (lateral && actual !== "index") {
        var h = '<a class="inicio' + (actual === "index" ? " actual" : "") + '" href="index.html">Inicio de la ayuda</a>';
        AYUDA.secciones.forEach(function (s) {
            var abierta = s.temas.some(function (t) { return t.id === actual; });
            h += '<details' + (abierta ? " open" : "") + '><summary>' + esc(s.titulo) +
                ' <span class="cant">' + s.temas.length + '</span></summary><ul>';
            s.temas.forEach(function (t) {
                if (t.listo === false) {
                    h += '<li><span class="pronto">' + esc(t.titulo) + '</span></li>';
                } else {
                    h += '<li><a href="' + t.id + '.html"' + (t.id === actual ? ' class="actual" aria-current="page"' : "") + '>' + esc(t.titulo) + '</a></li>';
                }
            });
            h += "</ul></details>";
        });
        lateral.innerHTML = h;
        var act = lateral.querySelector("a.actual");
        if (act && act.scrollIntoView) { act.scrollIntoView({ block: "nearest" }); }
    }

    var btnMenu = document.getElementById("menu-btn");
    if (btnMenu) {
        btnMenu.addEventListener("click", function () {
            var abierto = document.body.classList.toggle("lateral-abierto");
            btnMenu.setAttribute("aria-expanded", abierto ? "true" : "false");
        });
    }

    /* ---------- Buscador ---------- */
    var entrada = document.getElementById("buscar");
    var caja = document.getElementById("resultados");
    if (entrada && caja) {
        var indice = [];
        AYUDA.secciones.forEach(function (s) {
            s.temas.forEach(function (t) {
                if (t.listo === false) { return; }
                indice.push({
                    t: t, seccion: s.titulo,
                    texto: norm([t.titulo, t.resumen, (t.palabras || []).join(" "), (t.controles || []).join(" "), s.titulo].join(" "))
                });
            });
        });
        var sel = -1;
        function buscar() {
            var q = norm(entrada.value).trim();
            sel = -1;
            if (q.length < 2) { caja.classList.remove("abierto"); caja.innerHTML = ""; return; }
            var palabras = q.split(/\s+/);
            var hallados = [];
            indice.forEach(function (it) {
                var puntos = 0;
                for (var i = 0; i < palabras.length; i++) {
                    if (it.texto.indexOf(palabras[i]) === -1) { puntos = -1; break; }
                    puntos += norm(it.t.titulo).indexOf(palabras[i]) !== -1 ? 3 : 1;
                }
                if (puntos > 0) { hallados.push({ it: it, p: puntos }); }
            });
            hallados.sort(function (a, b) { return b.p - a.p; });
            if (!hallados.length) {
                caja.innerHTML = '<div class="vacio">No encontramos nada con "' + esc(entrada.value) + '". Probá con otra palabra.</div>';
            } else {
                caja.innerHTML = hallados.slice(0, 8).map(function (x) {
                    return '<a href="' + x.it.t.id + '.html"><b>' + esc(x.it.t.titulo) + '</b><small>' +
                        esc(x.it.seccion) + " · " + esc(x.it.t.resumen) + "</small></a>";
                }).join("");
            }
            caja.classList.add("abierto");
        }
        entrada.addEventListener("input", buscar);
        entrada.addEventListener("keydown", function (e) {
            var items = caja.querySelectorAll("a");
            if (e.key === "ArrowDown" || e.key === "ArrowUp") {
                e.preventDefault();
                if (!items.length) { return; }
                sel = (sel + (e.key === "ArrowDown" ? 1 : -1) + items.length) % items.length;
                for (var i = 0; i < items.length; i++) { items[i].classList.toggle("sel", i === sel); }
            } else if (e.key === "Enter") {
                var objetivo = items[sel >= 0 ? sel : 0];
                if (objetivo) { window.location.href = objetivo.getAttribute("href"); }
            } else if (e.key === "Escape") {
                caja.classList.remove("abierto"); entrada.blur();
            }
        });
        document.addEventListener("click", function (e) {
            if (!caja.contains(e.target) && e.target !== entrada) { caja.classList.remove("abierto"); }
        });
        document.addEventListener("keydown", function (e) {
            if (e.key === "/" && document.activeElement.tagName !== "INPUT") { e.preventDefault(); entrada.focus(); }
        });
    }

    /* ---------- Captura interactiva ---------- */
    var pantalla = document.querySelector(".pantalla");
    if (pantalla) {
        var globo = pantalla.querySelector(".globo");
        var zonas = pantalla.querySelectorAll(".hs");
        var lista = document.querySelectorAll(".elementos li");
        var fija = null;

        function datos(n) {
            var li = document.querySelector('.elementos li[data-n="' + n + '"]');
            if (!li) { return null; }
            return {
                titulo: li.querySelector("b").textContent,
                texto: li.querySelector("div > span").textContent,
                nota: li.getAttribute("data-nota") || ""
            };
        }
        function ubicar(zona) {
            var pr = pantalla.getBoundingClientRect(), zr = zona.getBoundingClientRect();
            var w = globo.offsetWidth, hGlobo = globo.offsetHeight;
            var x = zr.left - pr.left + zr.width / 2 - w / 2;
            x = Math.max(8, Math.min(x, pr.width - w - 8));
            var y = zr.bottom - pr.top + 12;
            if (y + hGlobo > pr.height - 6) { y = zr.top - pr.top - hGlobo - 12; }
            if (y < 6) { y = Math.min(pr.height - hGlobo - 6, zr.bottom - pr.top + 12); y = Math.max(6, y); }
            globo.style.left = x + "px";
            globo.style.top = y + "px";
        }
        function mostrar(zona) {
            var d = datos(zona.getAttribute("data-n"));
            if (!d) { return; }
            globo.innerHTML = '<strong><span class="g-num">' + esc(zona.getAttribute("data-n")) + "</span>" + esc(d.titulo) + "</strong><p>" +
                esc(d.texto) + "</p>" + (d.nota ? '<div class="g-nota">' + esc(d.nota) + "</div>" : "");
            ubicar(zona);
            globo.classList.add("visible");
            marcar(zona.getAttribute("data-n"));
        }
        function ocultar() {
            globo.classList.remove("visible");
            marcar(null);
        }
        function marcar(n) {
            for (var i = 0; i < zonas.length; i++) { zonas[i].classList.toggle("activo", zonas[i].getAttribute("data-n") === n); }
            for (var j = 0; j < lista.length; j++) { lista[j].classList.toggle("activo", lista[j].getAttribute("data-n") === n); }
        }

        Array.prototype.forEach.call(zonas, function (z) {
            z.addEventListener("mouseenter", function () { if (!fija) { mostrar(z); } });
            z.addEventListener("mouseleave", function () { if (!fija) { ocultar(); } });
            z.addEventListener("focus", function () { mostrar(z); });
            z.addEventListener("blur", function () { if (!fija) { ocultar(); } });
            z.addEventListener("click", function (e) {
                e.preventDefault();
                if (fija === z) { fija = null; ocultar(); } else { fija = z; mostrar(z); }
            });
        });
        pantalla.addEventListener("click", function (e) {
            if (e.target === pantalla || e.target.tagName === "IMG") { fija = null; ocultar(); }
        });
        document.addEventListener("keydown", function (e) { if (e.key === "Escape") { fija = null; ocultar(); } });

        Array.prototype.forEach.call(lista, function (li) {
            var n = li.getAttribute("data-n");
            var zona = pantalla.querySelector('.hs[data-n="' + n + '"]');
            if (!zona) { return; }
            li.addEventListener("mouseenter", function () { if (!fija) { mostrar(zona); } });
            li.addEventListener("mouseleave", function () { if (!fija) { ocultar(); } });
            li.addEventListener("click", function () {
                pantalla.scrollIntoView({ behavior: "smooth", block: "center" });
                fija = zona; mostrar(zona);
            });
        });

        var interruptor = document.getElementById("marcas");
        if (interruptor) {
            pantalla.classList.add("marcas-on");
            interruptor.addEventListener("click", function () {
                var on = pantalla.classList.toggle("marcas-on");
                interruptor.setAttribute("aria-pressed", on ? "true" : "false");
            });
        }
        window.addEventListener("resize", function () { if (fija) { ubicar(fija); } });
    }

    /* ---------- Pestanas de tareas ---------- */
    document.querySelectorAll(".tareas").forEach(function (cont) {
        var tabs = cont.querySelectorAll('[role="tab"]');
        var paneles = cont.querySelectorAll('[role="tabpanel"]');
        function activar(tab, enfocar) {
            Array.prototype.forEach.call(tabs, function (t, i) {
                var on = t === tab;
                t.setAttribute("aria-selected", on ? "true" : "false");
                t.tabIndex = on ? 0 : -1;
                paneles[i].hidden = !on;
            });
            if (enfocar) { tab.focus(); }
        }
        var m = /^#tab-(\d+)$/.exec(window.location.hash);
        if (m && tabs[+m[1]]) {
            activar(tabs[+m[1]], false);
            setTimeout(function () { cont.scrollIntoView({ block: "start" }); }, 60);
        }
        Array.prototype.forEach.call(tabs, function (t, i) {
            t.addEventListener("click", function () { activar(t, false); });
            t.addEventListener("keydown", function (e) {
                var k = null;
                if (e.key === "ArrowRight") { k = (i + 1) % tabs.length; }
                if (e.key === "ArrowLeft") { k = (i - 1 + tabs.length) % tabs.length; }
                if (k !== null) { e.preventDefault(); activar(tabs[k], true); }
            });
        });
    });
})();
