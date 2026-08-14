const splitMethod = document.querySelector("[data-split-method]");
const shareInput = document.querySelector("[data-share-input]");
const shareHint = document.querySelector("[data-share-hint]");

function refreshShareHint() {
  if (!splitMethod || !shareInput || !shareHint) {
    return;
  }

  const method = splitMethod.value;
  shareInput.disabled = method === "Equal";

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
