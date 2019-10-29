/**
 * Throttle function that catches and triggers last invocation.
 * Uses time to see if there is a last invocation.
 *
 * @export
 * @param {*} func
 * @param {*} limit
 * @returns
 */
export function throttleFunction(func, limit) {
  let lastFunc;
  let lastRan;
  return function () {
    // tslint:disable-next-line:no-this-assignment
    const context = this;
    const args = arguments;
    if (!lastRan) {
      func.apply(context, args);
      lastRan = Date.now();
    } else {
      clearTimeout(lastFunc);
      lastFunc = setTimeout(() => {
        if ((Date.now() - lastRan) >= limit) {
          func.apply(context, args);
          lastRan = Date.now();
        }
      },                    limit - (Date.now() - lastRan));
    }
  };
}
