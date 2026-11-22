(function () {
  'use strict';
  var root = document.documentElement;
  var KEY = 'stockledger-theme';

  // ---- theme switcher ----
  function markTheme() {
    document.querySelectorAll('[data-set-theme]').forEach(function (b) {
      b.classList.toggle('on', b.dataset.setTheme === root.dataset.theme);
    });
  }
  document.querySelectorAll('[data-set-theme]').forEach(function (b) {
    b.addEventListener('click', function () {
      root.dataset.theme = b.dataset.setTheme;
      try { localStorage.setItem(KEY, b.dataset.setTheme); } catch (e) { }
      markTheme();
    });
  });
  markTheme();

  // ---- count-up numbers ----
  var still = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  document.querySelectorAll('[data-count]').forEach(function (el) {
    var target = parseFloat(el.dataset.count) || 0;
    var decimals = parseInt(el.dataset.decimals || '0', 10);
    var prefix = el.dataset.prefix || '';
    function show(v) {
      el.textContent = prefix + v.toLocaleString('en-US', { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
    }
    if (still) { show(target); return; }
    var start = null, dur = 1100;
    function tick(ts) {
      if (start === null) start = ts;
      var p = Math.min((ts - start) / dur, 1);
      show(target * (1 - Math.pow(1 - p, 3)));
      if (p < 1) requestAnimationFrame(tick);
    }
    requestAnimationFrame(tick);
  });

  // ---- confirm before removing a product ----
  document.querySelectorAll('form[data-confirm]').forEach(function (f) {
    f.addEventListener('submit', function (e) {
      if (!window.confirm(f.dataset.confirm)) e.preventDefault();
    });
  });

  // ---- gentle reveal for cards below the fold ----
  if ('IntersectionObserver' in window && !still) {
    var io = new IntersectionObserver(function (entries) {
      entries.forEach(function (en) { if (en.isIntersecting) { en.target.classList.add('in'); io.unobserve(en.target); } });
    }, { threshold: .12 });
    document.querySelectorAll('.card, .table-wrap').forEach(function (el) {
      if (el.getBoundingClientRect().top > window.innerHeight) { el.classList.add('reveal'); io.observe(el); }
    });
  }
})();
