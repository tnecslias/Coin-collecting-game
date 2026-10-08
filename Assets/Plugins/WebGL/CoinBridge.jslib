mergeInto(LibraryManager.library, {
 ReportCoins: function(score, remaining, total) {
  if(typeof window.onCoinsChanged === 'function') window.onCoinsChanged(score, remaining, total);
 }
});
