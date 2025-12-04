import axios from 'axios';

// Config Defaults - הגדרת כתובת ברירת מחדל ל-API
axios.defaults.baseURL = "http://localhost:5223";

// Interceptor לטיפול בשגיאות
axios.interceptors.response.use(
  response => response,
  error => {
    console.error("API Error:", error.response?.data || error.message);
    return Promise.reject(error);
  }
);

const service = {
  getTasks: async () => {
    const result = await axios.get("/tasks");
    return result.data;
  },

  addTask: async (name) => {
    const result = await axios.post("/tasks", { name, isComplete: false });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    // קריאה ל-API כדי לקבל את המשימה הנוכחית
    const task = await axios.get(`/tasks`);
    const currentTask = task.data.find(t => t.id === id);
    
    if (!currentTask) {
      throw new Error(`Task with id ${id} not found`);
    }
    
    // שליחת כל הנתונים הנדרשים
    await axios.put(`/tasks/${id}`, { 
      name: currentTask.name,
      isComplete 
    });
  },

  deleteTask: async (id) => {
    await axios.delete(`/tasks/${id}`);
  }
};

export default service;
