const state = {
    mode: "login",
    auth: readAuth(),
    events: [],
    bookings: []
};

const statusNames = {
    1: "Draft",
    2: "Published",
    3: "Cancelled",
    4: "Completed"
};

const bookingStatusNames = {
    1: "Active",
    2: "Cancelled"
};

const els = {
    authBadge: document.querySelector("#authBadge"),
    loginTab: document.querySelector("#loginTab"),
    registerTab: document.querySelector("#registerTab"),
    authForm: document.querySelector("#authForm"),
    authSubmit: document.querySelector("#authSubmit"),
    nameField: document.querySelector("#nameField"),
    roleField: document.querySelector("#roleField"),
    fullName: document.querySelector("#fullName"),
    email: document.querySelector("#email"),
    password: document.querySelector("#password"),
    role: document.querySelector("#role"),
    profile: document.querySelector("#profile"),
    profileName: document.querySelector("#profileName"),
    profileEmail: document.querySelector("#profileEmail"),
    logoutBtn: document.querySelector("#logoutBtn"),
    refreshBtn: document.querySelector("#refreshBtn"),
    apiStatus: document.querySelector("#apiStatus"),
    adminSection: document.querySelector("#adminSection"),
    eventForm: document.querySelector("#eventForm"),
    eventsList: document.querySelector("#eventsList"),
    bookingsList: document.querySelector("#bookingsList"),
    eventListBadge: document.querySelector("#eventListBadge"),
    bookingListBadge: document.querySelector("#bookingListBadge"),
    eventsCount: document.querySelector("#eventsCount"),
    publishedCount: document.querySelector("#publishedCount"),
    bookingsCount: document.querySelector("#bookingsCount"),
    seatsCount: document.querySelector("#seatsCount"),
    toast: document.querySelector("#toast")
};

els.loginTab.addEventListener("click", () => setMode("login"));
els.registerTab.addEventListener("click", () => setMode("register"));
els.authForm.addEventListener("submit", submitAuth);
els.logoutBtn.addEventListener("click", logout);
els.refreshBtn.addEventListener("click", loadAll);
els.eventForm.addEventListener("submit", createEvent);

document.addEventListener("click", (event) => {
    const action = event.target.dataset.action;
    const id = Number(event.target.dataset.id);

    if (!action || !id) return;

    if (action === "book") {
        const input = document.querySelector(`#seats-${id}`);
        createBooking(id, Number(input?.value || 1));
    }

    if (action === "publish") publishEvent(id);
    if (action === "cancel-event") cancelEvent(id);
    if (action === "delete-event") deleteEvent(id);
    if (action === "cancel-booking") cancelBooking(id);
});

render();
if (state.auth?.token) {
    loadAll();
}

function setMode(mode) {
    state.mode = mode;
    renderAuthMode();
}

async function submitAuth(event) {
    event.preventDefault();

    const isRegister = state.mode === "register";
    const payload = {
        email: els.email.value.trim(),
        password: els.password.value
    };

    if (isRegister) {
        payload.fullName = els.fullName.value.trim();
        payload.role = Number(els.role.value);
    }

    try {
        const auth = await api(`/api/Auth/${isRegister ? "register" : "login"}`, {
            method: "POST",
            body: payload,
            skipAuth: true
        });

        state.auth = auth;
        localStorage.setItem("eventBookingAuth", JSON.stringify(auth));
        toast(`${isRegister ? "Registered" : "Logged in"} as ${roleName(auth.role)}.`);
        await loadAll();
        render();
    } catch (error) {
        toast(error.message);
    }
}

function logout() {
    state.auth = null;
    state.events = [];
    state.bookings = [];
    localStorage.removeItem("eventBookingAuth");
    render();
    toast("Logged out.");
}

async function loadAll() {
    if (!state.auth?.token) {
        render();
        return;
    }

    els.apiStatus.textContent = "Loading";

    try {
        const [events, bookings] = await Promise.all([
            api("/api/Event?pageNumber=1&pageSize=50"),
            api("/api/Booking?page=1&pageSize=50")
        ]);

        state.events = events.items || [];
        state.bookings = bookings.items || [];
        els.apiStatus.textContent = "Online";
        render();
    } catch (error) {
        els.apiStatus.textContent = "Error";
        toast(error.message);
        render();
    }
}

async function createEvent(event) {
    event.preventDefault();

    const seats = Number(document.querySelector("#eventSeats").value);
    const payload = {
        title: document.querySelector("#eventTitle").value.trim(),
        description: document.querySelector("#eventDescription").value.trim(),
        location: document.querySelector("#eventLocation").value.trim(),
        totalSeats: seats,
        availableSeats: seats,
        status: 2,
        eventDate: new Date(document.querySelector("#eventDate").value).toISOString(),
        price: Number(document.querySelector("#eventPrice").value)
    };

    try {
        await api("/api/Event", { method: "POST", body: payload });
        els.eventForm.reset();
        document.querySelector("#eventSeats").value = 40;
        document.querySelector("#eventPrice").value = 25;
        toast("Event created.");
        await loadAll();
    } catch (error) {
        toast(error.message);
    }
}

async function createBooking(eventId, seatsCount) {
    try {
        await api("/api/Booking", {
            method: "POST",
            body: {
                eventId,
                seatsCount,
                totalPrice: 0,
                status: 1
            }
        });

        toast("Booking created.");
        await loadAll();
    } catch (error) {
        toast(error.message);
    }
}

async function publishEvent(id) {
    await runAction(() => api(`/api/Event/${id}/publish`, { method: "PATCH" }), "Event published.");
}

async function cancelEvent(id) {
    await runAction(() => api(`/api/Event/${id}/cancel`, { method: "PATCH" }), "Event cancelled.");
}

async function deleteEvent(id) {
    await runAction(() => api(`/api/Event/${id}`, { method: "DELETE" }), "Event deleted.");
}

async function cancelBooking(id) {
    await runAction(() => api(`/api/Booking/${id}/cancel`, { method: "PATCH" }), "Booking cancelled.");
}

async function runAction(fn, message) {
    try {
        await fn();
        toast(message);
        await loadAll();
    } catch (error) {
        toast(error.message);
    }
}

async function api(path, options = {}) {
    const headers = {
        Accept: "application/json"
    };

    if (options.body) headers["Content-Type"] = "application/json";
    if (!options.skipAuth && state.auth?.token) {
        headers.Authorization = `Bearer ${state.auth.token}`;
    }

    const response = await fetch(path, {
        method: options.method || "GET",
        headers,
        body: options.body ? JSON.stringify(options.body) : undefined
    });

    if (!response.ok) {
        throw new Error(await readError(response));
    }

    if (response.status === 204) return null;

    const text = await response.text();
    return text ? JSON.parse(text) : null;
}

async function readError(response) {
    const text = await response.text();
    if (!text) return `${response.status} ${response.statusText}`;

    try {
        const data = JSON.parse(text);
        if (data.title) return data.title;
        if (data.detail) return data.detail;
        if (data.errors) return Object.values(data.errors).flat().join(" ");
        if (typeof data === "string") return data;
        return JSON.stringify(data);
    } catch {
        return text;
    }
}

function render() {
    renderAuthMode();
    renderProfile();
    renderStats();
    renderEvents();
    renderBookings();
}

function renderAuthMode() {
    const isRegister = state.mode === "register";
    els.loginTab.classList.toggle("active", !isRegister);
    els.registerTab.classList.toggle("active", isRegister);
    els.nameField.classList.toggle("hidden", !isRegister);
    els.roleField.classList.toggle("hidden", !isRegister);
    els.authSubmit.textContent = isRegister ? "Create account" : "Login";
}

function renderProfile() {
    const isLoggedIn = Boolean(state.auth?.token);
    els.authForm.classList.toggle("hidden", isLoggedIn);
    els.profile.classList.toggle("hidden", !isLoggedIn);
    els.adminSection.classList.toggle("hidden", !isAdmin());
    els.authBadge.textContent = isLoggedIn ? roleName(state.auth.role) : "Guest";

    if (isLoggedIn) {
        els.profileName.textContent = state.auth.fullName || "Signed user";
        els.profileEmail.textContent = state.auth.email || "";
    }
}

function renderStats() {
    const published = state.events.filter((item) => Number(item.status) === 2).length;
    const seats = state.events.reduce((sum, item) => sum + Number(item.availableSeats || 0), 0);

    els.eventsCount.textContent = state.events.length;
    els.publishedCount.textContent = published;
    els.bookingsCount.textContent = state.bookings.length;
    els.seatsCount.textContent = seats;
}

function renderEvents() {
    els.eventListBadge.textContent = state.auth?.token ? `${state.events.length} loaded` : "Login required";

    if (!state.auth?.token) {
        els.eventsList.className = "list empty-state";
        els.eventsList.textContent = "Login to load events.";
        return;
    }

    if (!state.events.length) {
        els.eventsList.className = "list empty-state";
        els.eventsList.textContent = "No events yet. Register as Admin and create the first one.";
        return;
    }

    els.eventsList.className = "list";
    els.eventsList.innerHTML = state.events.map((item) => eventTemplate(item)).join("");
}

function renderBookings() {
    els.bookingListBadge.textContent = state.auth?.token ? `${state.bookings.length} loaded` : "Login required";

    if (!state.auth?.token) {
        els.bookingsList.className = "list empty-state";
        els.bookingsList.textContent = "Your bookings will appear here.";
        return;
    }

    if (!state.bookings.length) {
        els.bookingsList.className = "list empty-state";
        els.bookingsList.textContent = "No bookings yet.";
        return;
    }

    els.bookingsList.className = "list";
    els.bookingsList.innerHTML = state.bookings.map((item) => bookingTemplate(item)).join("");
}

function eventTemplate(item) {
    const status = Number(item.status);
    const canBook = status === 2 && Number(item.availableSeats) > 0;
    const adminActions = isAdmin()
        ? `<button class="ghost" data-action="publish" data-id="${item.id}" type="button">Publish</button>
           <button class="danger" data-action="cancel-event" data-id="${item.id}" type="button">Cancel</button>
           <button class="danger" data-action="delete-event" data-id="${item.id}" type="button">Delete</button>`
        : "";

    return `
        <article class="item">
            <div class="item-head">
                <div>
                    <h3>${escapeHtml(item.title)}</h3>
                    <p>${escapeHtml(item.description)}</p>
                </div>
                <span class="badge ${statusClass(status)}">${statusNames[status] || "Unknown"}</span>
            </div>
            <div class="meta">
                <span>${formatDate(item.eventDate)}</span>
                <span>${escapeHtml(item.location)}</span>
                <span>${money(item.price)}</span>
                <span>${item.availableSeats}/${item.totalSeats} seats</span>
            </div>
            <div class="actions">
                <input id="seats-${item.id}" type="number" min="1" max="${Math.max(1, item.availableSeats)}" value="1" aria-label="Seats">
                <button class="primary" data-action="book" data-id="${item.id}" type="button" ${canBook ? "" : "disabled"}>Book</button>
                ${adminActions}
            </div>
        </article>`;
}

function bookingTemplate(item) {
    const status = Number(item.status);
    const eventTitle = item.event?.title || `Event #${item.eventId}`;
    const canCancel = status === 1;

    return `
        <article class="item">
            <div class="item-head">
                <div>
                    <h3>${escapeHtml(eventTitle)}</h3>
                    <p>${item.seatsCount} seat(s), ${money(item.totalPrice)}</p>
                </div>
                <span class="badge ${status === 2 ? "status-cancelled" : "status-published"}">${bookingStatusNames[status] || "Unknown"}</span>
            </div>
            <div class="meta">
                <span>Booking #${item.id}</span>
                <span>${formatDate(item.createdAt)}</span>
            </div>
            <div class="actions">
                <button class="danger" data-action="cancel-booking" data-id="${item.id}" type="button" ${canCancel ? "" : "disabled"}>Cancel booking</button>
            </div>
        </article>`;
}

function statusClass(status) {
    if (status === 2) return "status-published";
    if (status === 3) return "status-cancelled";
    return "status-draft";
}

function isAdmin() {
    return Number(state.auth?.role) === 2 || state.auth?.role === "Admin";
}

function roleName(role) {
    return Number(role) === 2 || role === "Admin" ? "Admin" : "User";
}

function readAuth() {
    try {
        return JSON.parse(localStorage.getItem("eventBookingAuth"));
    } catch {
        return null;
    }
}

function toast(message) {
    els.toast.textContent = message;
    els.toast.classList.remove("hidden");
    window.clearTimeout(toast.timer);
    toast.timer = window.setTimeout(() => els.toast.classList.add("hidden"), 4200);
}

function formatDate(value) {
    if (!value) return "No date";
    return new Intl.DateTimeFormat(undefined, {
        dateStyle: "medium",
        timeStyle: "short"
    }).format(new Date(value));
}

function money(value) {
    return new Intl.NumberFormat(undefined, {
        style: "currency",
        currency: "USD"
    }).format(Number(value || 0));
}

function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}
