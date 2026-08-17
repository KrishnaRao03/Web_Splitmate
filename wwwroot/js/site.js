const splitMethod = document.querySelector("[data-split-method]");
const shareInput = document.querySelector("[data-share-input]");
const shareHint = document.querySelector("[data-share-hint]");

function refreshShareHint() {
  if (!splitMethod || !shareInput) {
    return;
  }

  const method = splitMethod.value;
  shareInput.disabled = method === "Equal";

  if (!shareHint) {
    return;
  }

  if (method === "Equal") {
    shareInput.value = "";
    shareHint.textContent = "Equal split does not need custom shares.";
  } else if (method === "Exact") {
    shareHint.textContent = "Enter dollar amounts with the current member names, for example: Name=12, Name=18";
  } else {
    shareHint.textContent = "Enter percentages with the current member names. The total must equal 100.";
  }
}

splitMethod?.addEventListener("change", refreshShareHint);
refreshShareHint();

document.querySelectorAll("[data-checklist] input[type='checkbox']").forEach((checkbox, index) => {
  const key = `splitmate-demo-check-${index}`;
  checkbox.checked = localStorage.getItem(key) === "true";
  checkbox.addEventListener("change", () => localStorage.setItem(key, checkbox.checked));
});

function refreshSplitmateNav() {
  const items = document.querySelectorAll("[data-nav-key]");
  if (!items.length) {
    return;
  }

  const path = window.location.pathname.toLowerCase();
  const hash = window.location.hash.toLowerCase();
  let activeKey = "";

  if (path === "/" || path.endsWith("/home") || path.endsWith("/home/index")) {
    activeKey = "home";
  } else if (path.includes("/home/expenses") || path.includes("/home/balances")) {
    activeKey = "split";
  } else if (path.includes("/home/notestasks")) {
    activeKey = hash === "#notes" ? "notes" : "tasks";
  }

  items.forEach((item) => item.classList.toggle("active", item.dataset.navKey === activeKey));
}

window.addEventListener("hashchange", refreshSplitmateNav);
refreshSplitmateNav();
